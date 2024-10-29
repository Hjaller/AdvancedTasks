using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdvancedTasks.Utils
{
    public class Utils
    {
        public static int GetValidNumber(string prompt)
        {
            int number;
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (int.TryParse(input, out number))
                {
                    return number;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            }
        }

        public static double GetValidDecimal(string prompt)
        {
            double number;
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (double.TryParse(input, out number))
                {
                    return number;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid decimal number.");
                }
            }
        }

        public static string GetValidString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    return input;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a non-empty string.");
                }
            }
        }

        public static string GetValidEmail(string prompt)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (Regex.IsMatch(input, emailPattern))
                {
                    return input;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid email address.");
                }
            }
        }

        public static string GetValidPhoneNumber(string prompt)
        {
            string phonePattern = @"^\+?[1-9]\d{1,14}$"; // E.164 format
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (Regex.IsMatch(input, phonePattern))
                {
                    return input;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid phone number.");
                }
            }
        }
    }
}
