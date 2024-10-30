using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedTasks.EmployeeAndSalary
{
    public class Salary
    {
        public double Hours { get; set; }
        public double HourlySalary { get; set; }
        public double Taxes { get; set; }

        public Salary(double hours, double hourlySalary, double taxes)
        {
            Hours = hours;
            HourlySalary = hourlySalary;
            Taxes = taxes;
        }

        // Parameterløs konstruktør
        public Salary()
        {
            Hours = 0;
            HourlySalary = 0;
            Taxes = 0;
        }

        public double CalculateGrossSalary()
        {
            return Hours * HourlySalary;
        }

        public double CalculateNetSalary()
        {
            double grossSalary = CalculateGrossSalary();
            return grossSalary - (grossSalary * (Taxes / 100));
        }
    }
}
