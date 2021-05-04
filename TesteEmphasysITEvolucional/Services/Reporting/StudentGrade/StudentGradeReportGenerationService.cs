using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TesteEmphasysITEvolucional.Common;
using TesteEmphasysITEvolucional.Services.Data.ObjectModel;
using TesteEmphasysITEvolucional.Services.Reporting.ObjectModel;
using TesteEmphasysITEvolucional.Services.StudentGradeDataManagement;

namespace TesteEmphasysITEvolucional.Services.Reporting.StudentGrade
{
    public class StudentGradeReportGenerationService : IStudentGradeReportGenerationService
    {
        private readonly IStudentGradeDataManagementService _studentGradeDataManagementService;

        public StudentGradeReportGenerationService(IStudentGradeDataManagementService studentGradeDataManagementService)
        {
            _studentGradeDataManagementService = studentGradeDataManagementService ?? throw new ArgumentNullException("Students grades data management service argument cannot be null.");
        }

        public async Task<OperationResult<Report>> GenerateSchoolReportCardAsync(CancellationToken cancellationToken)
        {
            var studentsGrades = await _studentGradeDataManagementService.GetStudentsAndGradesAsync(cancellationToken);
            var report = new Report
            {
                Title = "All student's school report card",
                FileName = $"Notas{DateTime.Now:yyyyMMddhhmmss}.xls",
                ContentType = "application/vnd.ms-excel",
                DataBytes = Encoding.Default.GetBytes(GenerateReportContent(studentsGrades))
            };

            return OperationResult<Report>.Success(report);
        }

        private static string GenerateReportContent(IEnumerable<StudentGrades> studentsGrades)
        {
            if (studentsGrades == null || !studentsGrades.Any())
                return string.Empty;

            var sb = new StringBuilder();

            sb.Append("<table>");
            sb.Append("<tr>");
            sb.Append($"<td>Aluno</td>");
            foreach (var discipline in studentsGrades.First().Discipline_Grade.Keys)
            {
                sb.Append($"<td>{discipline}</td>");
            }
            sb.Append($"<td>Média</td>");
            sb.Append("</tr>");
            foreach (var studentGrade in studentsGrades)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{studentGrade.Student}</td>");
                foreach(var grade in studentGrade.Discipline_Grade)
                {
                    sb.Append($"<td>{grade.Value}</td>");
                }
                sb.AppendFormat("<td>{0}</td>", studentGrade.RoundAverage());
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            return $"{sb}";
        }
    }
}
