using Microsoft.Extensions.DependencyInjection;
using TesteEmphasysITEvolucional.Services.Data;
using TesteEmphasysITEvolucional.Services.Reporting.StudentGrade;
using TesteEmphasysITEvolucional.Services.Security.Authentication;
using TesteEmphasysITEvolucional.Services.StudentGradeDataManagement;

namespace TesteEmphasysITEvolucional.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserDataService, UserDataService>();
            services.AddScoped<IStudentGradeDataService, StudentGradeDataService>();
            services.AddScoped<IStudentGradeReportGenerationService, StudentGradeReportGenerationService>();
            services.AddScoped<IStudentGradeDataManagementService, StudentGradeDataManagementService>();

            return services;
        }
    }
}