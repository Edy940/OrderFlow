namespace OrderFlow.Api.Middleware
{
    public class ExceptionMiddleware
    {
       
            private readonly RequestDelegate _next;
            private readonly ILogger<ExceptionMiddleware> _logger;
            public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
            {
                _next = next;
                _logger = logger;
            }
            public async Task InvokeAsync(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ocorreu um erro não tratado.");
                    await HandleExceptionAsync(context, ex);
                }
            }
            private static Task HandleExceptionAsync(HttpContext context, Exception exception)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Ocorreu um erro interno no servidor.",
                    Detailed = exception.Message // Você pode remover isso em produção para não expor detalhes do erro
                };
                return context.Response.WriteAsJsonAsync(response);
            }
        }
    }
