using System.Threading;
using System.Threading.Tasks;
using TesteEmphasysITEvolucional.Common;
using TesteEmphasysITEvolucional.Services.Reporting.ObjectModel;

namespace TesteEmphasysITEvolucional.Services.Reporting.StudentGrade
{
    public interface IStudentGradeReportGenerationService
    {
        Task<OperationResult<Report>> GenerateSchoolReportCardAsync(CancellationToken cancellationToken);
    }
}