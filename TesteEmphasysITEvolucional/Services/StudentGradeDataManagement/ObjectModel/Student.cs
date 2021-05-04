using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TesteEmphasysITEvolucional.Services.StudentGradeDataManagement.ObjectModel
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override string ToString()
            => $"{FirstName} {LastName}";

        public class StudentComparer : IEqualityComparer<Student>
        {
            public bool Equals([AllowNull] Student x, [AllowNull] Student y)
                => $"{x}" == $"{y}";

            public int GetHashCode([DisallowNull] Student obj)
                => $"{obj}".GetHashCode() * 17;
        }
    }
}
