using CorrelationId;
using Microsoft.AspNetCore.Builder;
using OrderFlow.Api.Middleware;

namespace OrderFlow.Api.Extensions
{
    public static class CorrelationIdExtensions
    {
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CorrelationIdMiddleware>();
        }
    }
}