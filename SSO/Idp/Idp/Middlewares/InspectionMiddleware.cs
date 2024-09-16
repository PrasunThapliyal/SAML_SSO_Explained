using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Threading.Tasks;

namespace Idp.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class InspectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<InspectionMiddleware> _logger;

        public InspectionMiddleware(RequestDelegate next, ILogger<InspectionMiddleware> logger)
        {

            _next = next;
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.LogInformation($"Inside constructor of InspectionMiddleware ..");
        }

        public Task Invoke(HttpContext httpContext)
        {
            _logger.LogInformation($"Inside InspectionMiddleware.Invoke: {httpContext.Request.Method} {httpContext.Request.GetDisplayUrl()} ..");

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class InspectionMiddlewareExtensions
    {
        public static IApplicationBuilder UseInspectionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<InspectionMiddleware>();
        }
    }
}
