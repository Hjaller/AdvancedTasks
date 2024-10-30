﻿using AdvancedTasks.Phonebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdvancedTasks.EmployeeAndSalary
{
    public class EmployeeAndSalary
    {
        private static List<Employee> employees = new List<Employee>();
        private static readonly string filePath = "employee_salaries.json";
        private const int PageSize = 10;


        public static void CreateEmployee()
        {
            Console.Clear();
            string name = Utils.Utils.GetValidString("Enter the employees name: ");

            Employee employee = new Employee(name);
            employees.Add(employee);
            SaveEmployeesToFile();

            Console.Write("Employee created successfully.");

        }


        public static void EditEmployee(Employee employee)
        {
            Console.Clear();
            Console.WriteLine("Editing contact:");
            Console.WriteLine($"Current name: {employee.Name}");
            Console.Write("New name (leave blank to keep current): ");
            string newName = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(newName))
            {
                employee.Name = newName;
            }

            Console.WriteLine("Contact updated successfully.");
            ListOfEmployees();
        }

        public static void DeleteEmployee(Employee employee)
        {
            employees.Remove(employee);
            SaveEmployeesToFile();
            ListOfEmployees();
        }

        public static void AddSalaryToEmployee(Employee employee)
        {
            Console.Clear();
            double hours = Utils.Utils.GetValidDecimal("Enter the number of hours worked: ");
            double hourlySalary = Utils.Utils.GetValidDecimal("Enter the hourly salary: ");
            double taxes = Utils.Utils.GetValidDecimal("Enter the taxes procentage (without %): ");

            Salary newSalary = new Salary(hours, hourlySalary, taxes);
            employee.AddSalary(newSalary);
            SaveEmployeesToFile();

            Console.WriteLine("Salary added successfully.");
        }


        public static void ListOfEmployees(int pageNumber = 1, string searchTerm = "")
        {
            int selectedIndex = -1; // -1 indicates the search field
            StringBuilder searchTermBuilder = new StringBuilder(searchTerm);
            List<Employee> filteredEmployees = FilterEmployees(searchTermBuilder.ToString());

            int totalPages = (int)Math.Ceiling((double)filteredEmployees.Count / PageSize);
            if (pageNumber > totalPages) pageNumber = totalPages;
            if (pageNumber < 1) pageNumber = 1;

            Console.Clear();
            Console.WriteLine("Use the arrow keys to navigate,Enter to view and add salary, E to edit, and Delete to delete:");
            DisplaySearchField(searchTermBuilder.ToString(), selectedIndex);
            Console.WriteLine();

            if (filteredEmployees.Count == 0)
            {
                Console.WriteLine("No contacts found.");
            }
            else
            {
                DisplayEmployees(filteredEmployees, pageNumber, selectedIndex);
            }

            while (true)
            {
                var keyInfo = Console.ReadKey(intercept: true);
                int previousIndex = selectedIndex;

                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    if (selectedIndex == -1)
                    {
                        // Do nothing, already in search field
                    }
                    else
                    {
                        selectedIndex--;
                        if (selectedIndex < -1)
                        {
                            selectedIndex = -1;
                        }
                    }
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (selectedIndex == -1)
                    {
                        selectedIndex = 0;
                    }
                    else
                    {
                        selectedIndex++;
                        if (selectedIndex >= PageSize) selectedIndex = PageSize - 1;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.E)
                {
                    if (selectedIndex != -1)
                    {
                        int contactIndex = (pageNumber - 1) * PageSize + selectedIndex;
                        if (contactIndex < filteredEmployees.Count)
                        {
                            EditEmployee(filteredEmployees[contactIndex]);
                            DisplayEmployees(filteredEmployees, pageNumber, selectedIndex);
                        }
                    }
                    else
                    {
                        filteredEmployees = FilterEmployees(searchTermBuilder.ToString());
                        totalPages = (int)Math.Ceiling((double)filteredEmployees.Count / PageSize);
                        pageNumber = 1;
                        selectedIndex = -1;

                        Console.Clear();
                        Console.WriteLine("Use the arrow keys to navigate, Enter to edit, and Delete to delete:");
                        DisplaySearchField(searchTermBuilder.ToString(), selectedIndex);
                        Console.WriteLine();

                        if (filteredEmployees.Count == 0)
                        {
                            Console.WriteLine("No employees found.");
                        }
                        else
                        {
                            DisplayEmployees(filteredEmployees, pageNumber, selectedIndex);
                        }
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Delete)
                {
                    if (selectedIndex != -1)
                    {
                        int contactIndex = (pageNumber - 1) * PageSize + selectedIndex;
                        if (contactIndex < filteredEmployees.Count)
                        {
                            DeleteEmployee(filteredEmployees[contactIndex]);
                            filteredEmployees.RemoveAt(contactIndex);
                            DisplayEmployees(filteredEmployees, pageNumber, selectedIndex);
                        }
                    }
                }
                else if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (selectedIndex != -1 && pageNumber > 1)
                    {
                        Console.Clear();
                        pageNumber--;
                        selectedIndex = 0;
                        DisplayEmployees(filteredEmployees, pageNumber, selectedIndex);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    if (selectedIndex != -1 && pageNumber < totalPages)
                    {
                        Console.Clear();
                        pageNumber++;
                        selectedIndex = 0;
                        DisplayEmployees(filteredEmployees, pageNumber, selectedIndex);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    return;
                }
                else if (selectedIndex == -1)
                {
                    if (keyInfo.Key == ConsoleKey.Backspace && searchTermBuilder.Length > 0)
                    {
                        searchTermBuilder.Remove(searchTermBuilder.Length - 1, 1);
                    }
                    else if (!char.IsControl(keyInfo.KeyChar))
                    {
                        searchTermBuilder.Append(keyInfo.KeyChar);
                    }

                    DisplaySearchField(searchTermBuilder.ToString(), selectedIndex);
                }

                // Update only the changed lines
                if (previousIndex != selectedIndex)
                {
                    DisplaySearchField(searchTermBuilder.ToString(), selectedIndex);
                    DisplayEmployees(filteredEmployees, pageNumber, selectedIndex);
                }
            }
        }


        /// <summary>
        /// Filters the contact list based on the search term.
        /// </summary>
        /// <param name="searchTerm">The search term to filter contacts.</param>
        /// <returns>A list of filtered contacts.</returns>
        private static List<Employee> FilterEmployees(string searchTerm)
        {
            return employees
                .Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Displays a list of contacts for the current page.
        /// </summary>
        /// <param name="contacts">The list of contacts to display.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="selectedIndex">The index of the selected contact.</param>
        private static void DisplayEmployees(List<Employee> employee, int pageNumber, int selectedIndex)
        {
            Console.SetCursorPosition(0, 3); // Set cursor position to start of contact list
            int start = (pageNumber - 1) * PageSize;
            int end = Math.Min(start + PageSize, employee.Count);

            for (int i = start; i < end; i++)
            {
                if (i - start == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"> Nr. {i + 1} {employee[i].Name} - Salaries {employee[i].Salaries.Length}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  Nr. {i+1} {employee[i].Name} - Salaries {employee[i].Salaries.Length}");
                }
            }

            Console.WriteLine($"\nPage {pageNumber}/{(int)Math.Ceiling((double)employee.Count / PageSize)}");
        }

        /// <summary>
        /// Displays the search field with the current search term.
        /// </summary>
        /// <param name="searchTerm">The current search term.</param>
        /// <param name="selectedIndex">The index of the selected contact.</param>
        private static void DisplaySearchField(string searchTerm, int selectedIndex)
        {
            Console.SetCursorPosition(0, 1);
            Console.Write("Search: ");
            Console.ForegroundColor = selectedIndex == -1 ? ConsoleColor.Green : ConsoleColor.White;
            Console.WriteLine(searchTerm.PadRight(Console.WindowWidth - 8));
            Console.ResetColor();
        }


        /// <summary>
        /// Saves the contact list to a file.
        /// </summary>
        public static void SaveEmployeesToFile(bool sendMessage = false)
        {
            string json = JsonSerializer.Serialize(employees, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            if(sendMessage) Console.WriteLine("Contacts saved to file successfully.");
        }

        /// <summary>
        /// Loads the contact list from a file.
        /// </summary>
        public static void LoadEmployeesFromFile(bool sendMessage = false)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                employees = JsonSerializer.Deserialize<List<Employee>>(json) ?? new List<Employee>();
                if(sendMessage) Console.WriteLine("Employees loaded from file successfully.");
            }
            else
            {
                // Create the file if it does not exist
                File.WriteAllText(filePath, "[]");
                employees = new List<Employee>();
                if(sendMessage) Console.WriteLine("No employees file found. A new file has been created.");
            }
        }
    }
}