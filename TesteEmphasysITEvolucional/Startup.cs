using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using TesteEmphasysITEvolucional.Services;

namespace TesteEmphasysITEvolucional
{
    public class Startup
    {
        public static readonly string ApplicationName = "TesteEmphasysITEvolucional";

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(Environment.GetCommandLineArgs().Skip(1).ToArray());

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection(x => x.ApplicationDiscriminator = ApplicationName)
                    .SetApplicationName(ApplicationName)
                    .AddKeyManagementOptions(options =>
                    {
                        options.NewKeyLifetime = TimeSpan.FromDays(7);
                        options.AutoGenerateKeys = true;
                    });
            services.AddControllersWithViews();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie();
            services.AddServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime, IDataProtectionProvider protectionProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Strict, HttpOnly = HttpOnlyPolicy.Always });
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseDatabaseCreation(
                Configuration,
                appLifetime,
                protectionProvider.CreateProtector(ApplicationName),
                app.ApplicationServices.GetService<ILogger<Startup>>());
        }
    }
}
