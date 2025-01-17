using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PrintIt.Core;
using PrintIt.ServiceHost.Middlewares;

namespace PrintIt.ServiceHost
{
    [ExcludeFromCodeCoverage]
    internal sealed class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddPrintIt();

            services.AddRouting();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            app.UsePrintItAuthentication();
            app.UseAuthorization();
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });            
        }
    }
}
