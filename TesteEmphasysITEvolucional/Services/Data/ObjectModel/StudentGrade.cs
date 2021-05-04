using System;
using System.Collections.Generic;
using System.Linq;
using TesteEmphasysITEvolucional.Services.StudentGradeDataManagement.ObjectModel;

namespace TesteEmphasysITEvolucional.Services.Data.ObjectModel
{
    public class StudentGrades
    {
        public Student Student { get; set; }
        public IDictionary<string, double> Discipline_Grade { get; set; }
        public double Average { get => Discipline_Grade?.Values?.Average() ?? 0; }
        public string RoundAverage()
        {
            double roundedAverage = Average;
            double remainder;
            var integerPart = (int)Average;

            if ((remainder = Average % integerPart) != 0)
            {
                if (remainder > 0.5D)
                    roundedAverage = Math.Ceiling(Average);
                else if (remainder < 0.5D)
                    roundedAverage = Math.Floor(Average);
            }

            return $"{roundedAverage:##.##}";
        }
    }
}