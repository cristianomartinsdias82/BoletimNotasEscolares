using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TesteEmphasysITEvolucional.Common;
using TesteEmphasysITEvolucional.Services.Data.ObjectModel;

namespace TesteEmphasysITEvolucional.Services.StudentGradeDataManagement
{
    public interface IStudentGradeDataManagementService
    {
        Task<OperationResult> GenerateStudentsAndGradesDataLoadAsync(CancellationToken cancellationToken);
        Task<IEnumerable<StudentGrades>> GetStudentsAndGradesAsync(CancellationToken cancellationToken);
    }
}
