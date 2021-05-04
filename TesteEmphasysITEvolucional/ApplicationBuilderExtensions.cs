using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TesteEmphasysITEvolucional.Infrastructure.DatabaseCreation;

namespace TesteEmphasysITEvolucional
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDatabaseCreation(
            this IApplicationBuilder app,
            IConfiguration configuration,
            IHostApplicationLifetime appLifetime,
            IDataProtector dataProtector,
            ILogger<Startup> logger)
        {
            DatabaseCreationHelper.Run(configuration, appLifetime, dataProtector, logger);

            return app;
        }
    }
}
