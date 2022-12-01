using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Threading.Tasks;

namespace PrintIt.ServiceHost.Middlewares
{
    public class PrintItAuthentication
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _appSettings;

        public PrintItAuthentication(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _appSettings = config;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var auth = httpContext.Request.Headers["Authorization"];

            var token = _appSettings.GetValue<string>("Authorization:BearerToken");

            if (auth != token)
            {
                httpContext.Response.Clear();
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await httpContext.Response.WriteAsync("Bearer Token missing or invalid!");
                return;
            }

            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class PrintItAuthenticationExtensions
    {
        public static IApplicationBuilder UsePrintItAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PrintItAuthentication>();
        }
    }
}
