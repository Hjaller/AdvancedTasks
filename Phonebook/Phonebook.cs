using AdvancedTasks.Menu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace AdvancedTasks.Phonebook
{
    internal class Phonebook
    {
        private static List<Contact> contacts = new List<Contact>();
        private static readonly string filePath = "contacts.json";
        private const int PageSize = 10;

        /// <summary>
        /// Creates a new contact and adds it to the contact list.
        /// </summary>
        public static void CreateContact()
        {
            Console.Clear();
            string name = Utils.Utils.GetValidString("Enter the name of the contact: ");
            string phoneNumber = Utils.Utils.GetValidPhoneNumber("Enter the phone number of the contact: ");
            string email = Utils.Utils.GetValidEmail("Enter the email of the contact: ");

            Contact newContact = new Contact(name, phoneNumber, email);
            contacts.Add(newContact);

            Console.WriteLine("Contact created successfully.");
        }

        /// <summary>
        /// Edits an existing contact.
        /// </summary>
        /// <param name="contact">The contact to edit.</param>
        public static void EditContact(Contact contact)
        {
            Console.Clear();
            Console.WriteLine("Editing contact:");
            Console.WriteLine($"Current name: {contact.Name}");
            Console.Write("New name (leave blank to keep current): ");
            string newName = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(newName))
            {
                contact.Name = newName;
            }

            Console.WriteLine($"Current phone number: {contact.PhoneNumber}");
            Console.Write("New phone number (leave blank to keep current): ");
            string newPhoneNumber = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(newPhoneNumber))
            {
                contact.PhoneNumber = newPhoneNumber;
            }

            Console.WriteLine($"Current email: {contact.Email}");
            Console.Write("New email (leave blank to keep current): ");
            string newEmail = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(newEmail))
            {
                contact.Email = newEmail;
            }

            Console.WriteLine("Contact updated successfully.");
            ListOfContact();
        }

        /// <summary>
        /// Deletes a contact from the contact list.
        /// </summary>
        /// <param name="contact">The contact to delete.</param>
        public static void DeleteContact(Contact contact)
        {
            contacts.Remove(contact);
            Console.WriteLine("Contact deleted successfully.");
        }

        /// <summary>
        /// Displays a paginated list of contacts with search functionality.
        /// </summary>
        /// <param name="pageNumber">The page number to display.</param>
        /// <param name="searchTerm">The search term to filter contacts.</param>
        public static void ListOfContact(int pageNumber = 1, string searchTerm = "")
        {
            int selectedIndex = -1; // -1 indicates the search field
            StringBuilder searchTermBuilder = new StringBuilder(searchTerm);
            List<Contact> filteredContacts = FilterContacts(searchTermBuilder.ToString());

            int totalPages = (int)Math.Ceiling((double)filteredContacts.Count / PageSize);
            if (pageNumber > totalPages) pageNumber = totalPages;
            if (pageNumber < 1) pageNumber = 1;

            Console.Clear();
            Console.WriteLine("Use the arrow keys to navigate, Enter to edit, and Delete to delete:");
            DisplaySearchField(searchTermBuilder.ToString(), selectedIndex);
            Console.WriteLine();

            DisplayContacts(filteredContacts, pageNumber, selectedIndex);

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
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    if (selectedIndex != -1)
                    {
                        int contactIndex = (pageNumber - 1) * PageSize + selectedIndex;
                        if (contactIndex < filteredContacts.Count)
                        {
                            EditContact(filteredContacts[contactIndex]);
                            DisplayContacts(filteredContacts, pageNumber, selectedIndex);
                        }
                    }
                    else
                    {
                        filteredContacts = FilterContacts(searchTermBuilder.ToString());
                        totalPages = (int)Math.Ceiling((double)filteredContacts.Count / PageSize);
                        pageNumber = 1;
                        selectedIndex = -1;

                        Console.Clear();
                        Console.WriteLine("Use the arrow keys to navigate, Enter to edit, and Delete to delete:");
                        DisplaySearchField(searchTermBuilder.ToString(), selectedIndex);
                        Console.WriteLine();

                        DisplayContacts(filteredContacts, pageNumber, selectedIndex);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Delete)
                {
                    if (selectedIndex != -1)
                    {
                        int contactIndex = (pageNumber - 1) * PageSize + selectedIndex;
                        if (contactIndex < filteredContacts.Count)
                        {
                            DeleteContact(filteredContacts[contactIndex]);
                            filteredContacts.RemoveAt(contactIndex);
                            DisplayContacts(filteredContacts, pageNumber, selectedIndex);
                        }
                    }
                }
                else if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (selectedIndex != -1 && pageNumber > 1)
                    {
                        pageNumber--;
                        selectedIndex = 0;
                        DisplayContacts(filteredContacts, pageNumber, selectedIndex);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    if (selectedIndex != -1 && pageNumber < totalPages)
                    {
                        pageNumber++;
                        selectedIndex = 0;
                        DisplayContacts(filteredContacts, pageNumber, selectedIndex);
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
                    DisplayContacts(filteredContacts, pageNumber, selectedIndex);
                }
            }
        }

        /// <summary>
        /// Filters the contact list based on the search term.
        /// </summary>
        /// <param name="searchTerm">The search term to filter contacts.</param>
        /// <returns>A list of filtered contacts.</returns>
        private static List<Contact> FilterContacts(string searchTerm)
        {
            return contacts
                .Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            c.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Displays a list of contacts for the current page.
        /// </summary>
        /// <param name="contacts">The list of contacts to display.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="selectedIndex">The index of the selected contact.</param>
        private static void DisplayContacts(List<Contact> contacts, int pageNumber, int selectedIndex)
        {
            Console.SetCursorPosition(0, 3); // Set cursor position to start of contact list
            int start = (pageNumber - 1) * PageSize;
            int end = Math.Min(start + PageSize, contacts.Count);

            for (int i = start; i < end; i++)
            {
                if (i - start == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"> {contacts[i].Name} - {contacts[i].PhoneNumber} - {contacts[i].Email}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {contacts[i].Name} - {contacts[i].PhoneNumber} - {contacts[i].Email}");
                }
            }

            Console.WriteLine($"\nPage {pageNumber}/{(int)Math.Ceiling((double)contacts.Count / PageSize)}");
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
        /// Prompts the user to enter a search term and displays the filtered contact list.
        /// </summary>
        public static void SearchContacts()
        {
            Console.Write("Enter search term (name, phone number, or email): ");
            string searchTerm = Console.ReadLine() ?? string.Empty;
            ListOfContact(1, searchTerm);
        }

        /// <summary>
        /// Saves the contact list to a file.
        /// </summary>
        public static void SaveContactsToFile()
        {
            string json = JsonSerializer.Serialize(contacts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            Console.WriteLine("Contacts saved to file successfully.");
        }

        /// <summary>
        /// Loads the contact list from a file.
        /// </summary>
        public static void LoadContactsFromFile()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                contacts = JsonSerializer.Deserialize<List<Contact>>(json) ?? new List<Contact>();
                Console.WriteLine("Contacts loaded from file successfully.");
            }
            else
            {
                // Create the file if it does not exist
                File.WriteAllText(filePath, "[]");
                contacts = new List<Contact>();
                Console.WriteLine("No contacts file found. A new file has been created.");
            }
        }
    }
}
