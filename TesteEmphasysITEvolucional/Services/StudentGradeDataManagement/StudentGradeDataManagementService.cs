using Bogus;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TesteEmphasysITEvolucional.Common;
using TesteEmphasysITEvolucional.Services.Data;
using TesteEmphasysITEvolucional.Services.Data.ObjectModel;
using TesteEmphasysITEvolucional.Services.StudentGradeDataManagement.ObjectModel;
using Z.Dapper.Plus;

namespace TesteEmphasysITEvolucional.Services.StudentGradeDataManagement
{
    public class StudentGradeDataManagementService : IStudentGradeDataManagementService
    {
        private readonly IStudentGradeDataService _studentGradeDataService;
        private readonly string _connectionString;
        private readonly string _studentsGradesCacheKeyName = "STUDENTS_GRADES";
        private readonly IMemoryCache _cacheProvider;
        private readonly IList<string> _disciplines;

        public StudentGradeDataManagementService(
            IStudentGradeDataService studentGradeDataService,
            IMemoryCache cacheProvider,
            IConfiguration configuration)
        {
            _studentGradeDataService = studentGradeDataService ?? throw new ArgumentNullException("Student grade data service argument cannot be null.");
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException("Cache provider argument cannot be null.");
            _connectionString = configuration["ConnectionStrings:projeto-evolucional"];
            _disciplines = new List<string> { "Matemática", "Português", "História", "Geografia", "Inglês", "Biologia", "Filosofia", "Física", "Química" };
        }

        public async Task<OperationResult> GenerateStudentsAndGradesDataLoadAsync(CancellationToken cancellationToken)
        {
            if (_cacheProvider.Get(_studentsGradesCacheKeyName) != null)
                return OperationResult.Success();

            IDbTransaction transaction = default;
            DapperPlusContext context;
            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    var dataLoadedAlready = await conn.ExecuteScalarAsync(new CommandDefinition("SELECT 1 FROM dbo.StudentsGrades (NOLOCK)", cancellationToken: cancellationToken));
                    if (dataLoadedAlready != null)
                        return OperationResult.Success();

                    using (transaction = await conn.BeginTransactionAsync(cancellationToken))
                    {
                        context = new DapperPlusContext(transaction.Connection, transaction);

                        await RegisterDisciplines(context, cancellationToken);
                        await GenerateStudentsDataLoadAsync(context, cancellationToken);
                        await GenerateStudentsGradesDataLoadAsync(context, cancellationToken);

                        transaction.Commit();
                    }
                }
                catch (Exception exc)
                {
                    transaction?.Rollback();

                    return OperationResult.Failure($"Unable to create students and grades data load. {exc.Message}");
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                    context = null;
                }
            }

            return OperationResult.Success();
        }

        private async Task RegisterDisciplines(DapperPlusContext dataContext, CancellationToken cancellationToken)
        {
            var disciplines = _disciplines.Select(d => new Discipline { Title = d });

            dataContext.Entity<Discipline>()
               .Table($"{nameof(Discipline)}s")
               .Identity(x => x.Id);

            await dataContext.BulkActionAsync(x => x.BulkInsert(disciplines), cancellationToken);
        }

        private async Task GenerateStudentsGradesDataLoadAsync(DapperPlusContext dataContext, CancellationToken cancellationToken)
        {
            var sql =
            "INSERT dbo.StudentsGrades (StudentId, DisciplineId, Grade)" +
            "SELECT " +
            "     S.Id StudentId " +
            "    ,D.Id DisciplineId " +
            "    ,dbo.F_RandomValue() Grade " +
            "FROM dbo.Students S " +
            "CROSS JOIN dbo.Disciplines D";

            await dataContext.Connection.ExecuteAsync(sql, transaction: dataContext.Transaction);
        }

        private async Task GenerateStudentsDataLoadAsync(DapperPlusContext dataContext, CancellationToken cancellationToken)
        {
            const int studentCount = 1000;

            var studentsDataFaker = new Faker<Student>()
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName());

            var students = studentsDataFaker.Generate(studentCount).Distinct(new Student.StudentComparer());

            dataContext.Entity<Student>()
               .Table($"{nameof(Student)}s")
               .Identity(x => x.Id)
               .BatchSize(studentCount);

            await dataContext.BulkActionAsync(x => x.BulkInsert(students), cancellationToken);
        }

        public async Task<IEnumerable<StudentGrades>> GetStudentsAndGradesAsync(CancellationToken cancellationToken)
        {
            IEnumerable<StudentGrades> studentGrades;
            if (_cacheProvider.TryGetValue(_studentsGradesCacheKeyName, out studentGrades))
                return studentGrades;

            using (var conn = new SqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();

                    using (var reader = await conn.ExecuteReaderAsync("SELECT * FROM dbo.V_StudentsGrades ORDER BY Nome, Sobrenome"))
                    {
                        studentGrades = GetFromReader(reader);
                    }

                    _cacheProvider.Set(_studentsGradesCacheKeyName, studentGrades);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }

            return studentGrades;
        }

        private IEnumerable<StudentGrades> GetFromReader(DbDataReader reader)
        {
            IList<StudentGrades> studentsGrades = new List<StudentGrades>();

            while (reader?.Read() ?? false)
            {
                var grades = new StudentGrades
                {
                    Student = new Student { FirstName = reader.GetString("Nome"), LastName = reader.GetString("Sobrenome") },
                    Discipline_Grade = new Dictionary<string, double>()
                };
                _disciplines.ToList().ForEach(d => grades.Discipline_Grade.Add(d, Convert.ToDouble(reader[d])));

                studentsGrades.Add(grades);
            }

            return studentsGrades;
        }
    }
}
