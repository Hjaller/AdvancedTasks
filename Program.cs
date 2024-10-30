using AdvancedTasks.Menu;

namespace AdvancedTasks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Phonebook.Phonebook.LoadContactsFromFile();
                EmployeeAndSalary.EmployeeAndSalary.LoadEmployeesFromFile();
                ShowMenu(MainMenuOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                try
                {
                    Phonebook.Phonebook.SaveContactsToFile();
                    EmployeeAndSalary.EmployeeAndSalary.LoadEmployeesFromFile();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to save contacts: {ex.Message}");
                }
            }
        }


        static private MenuOption[] MainMenuOptions =
        {
                new MenuOption("Phonebook", null, new MenuOption[]
                {
                    new MenuOption("Create contact", Phonebook.Phonebook.CreateContact),
                    new MenuOption("List contacts", () => Phonebook.Phonebook.ListOfContact()),
                    new MenuOption("Save to file", Phonebook.Phonebook.SaveContactsToFile),// Fix applied here
                }),
                new MenuOption("Employee and Salary", null, new MenuOption[]
                {
                    new MenuOption("Create employee", EmployeeAndSalary.EmployeeAndSalary.CreateEmployee),
                    new MenuOption("List employees", () => EmployeeAndSalary.EmployeeAndSalary.ListOfEmployees()),
                    new MenuOption("Save to file", () => EmployeeAndSalary.EmployeeAndSalary.SaveEmployeesToFile()),// Fix applied here
                }),
                new MenuOption("Exit", ExitProgram)
            };

        static private void ShowMenu(MenuOption[] menuOptions)
        {
            int selectedIndex = 0;

            Console.Clear();
            Console.WriteLine("Use the arrow keys to navigate, and Enter to select an option:\n");

            // Initial rendering of the menu
            for (int i = 0; i < menuOptions.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"> {menuOptions[i].OptionText}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {menuOptions[i].OptionText}");
                }
            }

            while (true)
            {
                var keyInfo = Console.ReadKey(intercept: true);
                int previousIndex = selectedIndex;

                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    selectedIndex--;
                    if (selectedIndex < 0) selectedIndex = menuOptions.Length - 1;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    selectedIndex++;
                    if (selectedIndex >= menuOptions.Length) selectedIndex = 0;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    var selectedOption = menuOptions[selectedIndex];

                    if (selectedOption != null)
                    {
                        if (selectedOption.SubMenu != null)
                        {
                            ShowMenu(selectedOption.SubMenu);
                            Console.Clear();
                            Console.WriteLine("Use the arrow keys to navigate, and Enter to select an option:\n");
                            for (int i = 0; i < menuOptions.Length; i++)
                            {
                                if (i == selectedIndex)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"> {menuOptions[i].OptionText}");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.WriteLine($"  {menuOptions[i].OptionText}");
                                }
                            }
                        }
                        else if (selectedOption.Action != null)
                        {
                            Console.Clear();
                            selectedOption.Action.Invoke();
                            WaitForEnter(selectedOption.Action);
                            Console.Clear();
                            Console.WriteLine("Use the arrow keys to navigate, and Enter to select an option:\n");
                            for (int i = 0; i < menuOptions.Length; i++)
                            {
                                if (i == selectedIndex)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine($"> {menuOptions[i].OptionText}");
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.WriteLine($"  {menuOptions[i].OptionText}");
                                }
                            }
                        }
                        else if (selectedOption.OptionText.StartsWith("0"))
                        {
                            return; // Return to the previous menu
                        }
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    ExitProgram();
                    return;
                }

                // Update only the changed lines
                if (previousIndex != selectedIndex)
                {
                    Console.SetCursorPosition(0, previousIndex + 2); // +2 to account for the initial instructions
                    Console.Write($"  {menuOptions[previousIndex].OptionText}  ");

                    Console.SetCursorPosition(0, selectedIndex + 2);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"> {menuOptions[selectedIndex].OptionText}");
                    Console.ResetColor();
                }
            }
        }

        static private void WaitForEnter(Action action)
        {
            Console.WriteLine("\nPress Enter to run the task again or Esc to return to the main menu.");
            while (true)
            {
                var keyInfo = Console.ReadKey(intercept: true);
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    action.Invoke();
                    Console.WriteLine("\nPress Enter to run the task again or Esc to return to the main menu.");
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    return; // Return to the main menu
                }
            }
        }

        static private void ExitProgram()
        {
            Console.WriteLine("The program is exiting. Goodbye!");
            Phonebook.Phonebook.SaveContactsToFile();
            Environment.Exit(0);
        }
    }
}

