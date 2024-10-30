using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AdvancedTasks.EmployeeAndSalary
{
    public class Employee
    {
        public string Name { get; set; }
        public Salary[] Salaries { get; set; }

        [JsonConstructor]
        public Employee(string name, Salary[] salaries)
        {
            Name = name;
            Salaries = salaries;
        }

        public Employee(string name)
        {
            Name = name;
            Salaries = new Salary[0];
        }

        // Parameterløs konstruktør
        public Employee()
        {
            Name = string.Empty;
            Salaries = new Salary[0];
        }

        public void AddSalary(Salary salary)
        {
            var salariesList = Salaries?.ToList() ?? new List<Salary>();
            salariesList.Add(salary);
            Salaries = salariesList.ToArray();
        }
    }
}
