using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Domain.Exceptions;

namespace OrderFlow.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception) when (!context.Response.HasStarted)
        {
            await EscreverProblemaAsync(context, exception);
        }
    }

    private async Task EscreverProblemaAsync(HttpContext context, Exception exception)
    {
        var (status, titulo) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Falha de validação"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
            RecursoNaoEncontradoException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Não autorizado"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor")
        };

        if (status >= 500)
            _logger.LogError(exception, "Erro não tratado. TraceId={TraceId}", context.TraceIdentifier);
        else
            _logger.LogWarning(exception, "Requisição rejeitada. TraceId={TraceId}", context.TraceIdentifier);

        var problem = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = status < 500 || _environment.IsDevelopment()
                ? exception.Message
                : "Ocorreu um erro inesperado.",
            Instance = context.Request.Path,
            Type = $"https://httpstatuses.com/{status}"
        };
        problem.Extensions["traceId"] = context.TraceIdentifier;

        if (exception is ValidationException validationException)
        {
            problem.Extensions["errors"] = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        }

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(
            problem,
            options: null,
            contentType: "application/problem+json",
            cancellationToken: context.RequestAborted);
    }
}
