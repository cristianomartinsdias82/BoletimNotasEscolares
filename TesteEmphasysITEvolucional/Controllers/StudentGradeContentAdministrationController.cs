using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using TesteEmphasysITEvolucional.Services.Reporting.StudentGrade;
using TesteEmphasysITEvolucional.Services.StudentGradeDataManagement;

namespace TesteEmphasysITEvolucional.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class StudentGradeContentAdministrationController : Controller
    {
        public IActionResult Index()
            => View();

        [HttpPost]
        public async Task<IActionResult> LoadDatabase([FromServices] IStudentGradeDataManagementService studentGradeDataManagementService, CancellationToken cancellationToken)
        {
            await studentGradeDataManagementService.GenerateStudentsAndGradesDataLoadAsync(cancellationToken);

            TempData["DatabaseLoadingSuccessful"] = 1;

            return View(nameof(Index));
        }

        public async Task<IActionResult> DownloadReport([FromServices] IStudentGradeReportGenerationService studentGradeReportGenerationService, CancellationToken cancellationToken)
        {
            var reportGenerationResult = await studentGradeReportGenerationService.GenerateSchoolReportCardAsync(cancellationToken);

            if (!reportGenerationResult.Successful)
            {
                TempData["Message"] = reportGenerationResult.Message;
                return View(nameof(Index));
            }

            var report = reportGenerationResult.Data;
            return File(report.DataBytes, report.ContentType, report.FileName);
        }
    }
}
