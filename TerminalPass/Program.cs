using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json;
using System.Security;
using ConsoleTables;
using Alba.CsConsoleFormat.Fluent;

namespace TerminalPass
{
    class AccountRecord
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

    class Configuration
    {
        public bool Hide { get; set; }
        public Format TableFormat { get; set; } = Format.Alternative;
        public AccountRecord[] Accounts { get; set; } = new AccountRecord[] { };
    }

    class Program
    {
        public static string path = Environment.OSVersion.Version.Major >= 6
            ? Directory.GetParent(Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName).ToString()
            : Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
        public static string fileName = "terminal-pass.txt";
        public static string absolutePath = Path.Combine(path, fileName);

        public static string masterPassword;
        public static Configuration config;

        static void Main(string[] args)
        {
            while (true)
            {

                try
                {
                    using (var r = new StreamReader(absolutePath))
                    {
                        Console.Write("Password: ");
                        masterPassword = GetPassword();
                        Console.Clear();

                        string encoded = r.ReadToEnd();
                        string json = Aes.Decrypt(encoded, masterPassword);
                        config = JsonConvert.DeserializeObject<Configuration>(json);
                    }
                    EditConfig();
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("==========NEW LOGIN, CREATE NEW FILE==========");
                    Console.Write("Password: ");
                    string masterPassword = GetPassword();
                    Console.WriteLine();

                    Configuration config = new Configuration
                    {
                        Accounts = new[] {
                        new AccountRecord { Name = "Google", Password ="CanAamishPlease" },
                        new AccountRecord { Name = "Outlook", Password = "GetAnArrangedMarriange" },
                        new AccountRecord { Name = "Github", Password = "OrIWillShowHimDaWae" },
                    },
                    };

                    WriteConfig(config, masterPassword);
                }
                catch (JsonReaderException)
                {
                    Console.WriteLine("Error parsing the file");
                }
                catch (CryptographicException)
                {
                    Console.WriteLine("Invalid file or password");
                }

                // [x] if file not exists -> create one -> ask to create password
                // else -> while password not valid -> ask for password; allow to:
                //    view, add, delete rows
                //    search
            }
        }

        public static void EditConfig()
        {
            string c = "";
            while(c != "exit")
            {
                var accounts = new List<AccountRecord>();
                for (int i = 0; i < config.Accounts.Length; i++)
                {
                    var acc = new AccountRecord{
                        Name = config.Accounts[i].Name,
                        Password = config.Hide ? new string('*', config.Accounts[i].Password.Length) : config.Accounts[i].Password
                    };
                    accounts.Add(acc);

                }
                if (c != "help")
                    ConsoleTable.From(accounts).Write((Format)config.TableFormat);

                Console.Write("terminal-pass> ");
                string raw = Console.ReadLine();
                string[] tokens = raw.Split(' ').ToArray();
                try
                {
                    c = tokens[0];
                    switch (c)
                    {
                        case "help":
                            Colors.WriteLine("\treload".Yellow(), " -> ", "Updates the file and reloads the table");
                            Colors.WriteLine("\tdelete".Yellow(), "name".Green(), " -> ", "Deletes row with that name");
                            break;
                        case "reload":
                            WriteConfig(config, masterPassword);
                            Console.Clear();
                            break;
                        // delete name
                        case "delete":
                            string nameToDelete = tokens[1];
                            config.Accounts = config.Accounts.Where(acc => acc.Name != nameToDelete).ToArray();
                            WriteConfig(config, masterPassword);
                            Console.Clear();
                            break;
                        // add name password
                        case "add":
                            string name = tokens[1];
                            string password = tokens[2];
                            config.Accounts = config.Accounts
                                .Concat(new AccountRecord[] { new AccountRecord { Name = name, Password = password } })
                                .ToArray();
                            WriteConfig(config, masterPassword);
                            Console.Clear();
                            break;
                        case "set":
                            switch(tokens[1])
                            {
                                case "hide":
                                    if (bool.TryParse(tokens[2], out bool hide))
                                    {
                                        config.Hide = hide;
                                    }
                                    break;
                            }
                            WriteConfig(config, masterPassword);
                            Console.Clear();
                            break;
                        default:
                            Console.WriteLine($"Invalid name {tokens[1]}.");
                            break;
                    }
                } catch(Exception e)
                {
                    Console.WriteLine("Invalid command");
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static string GetPassword()
        {
            string pwd = "";
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd = pwd.Remove(pwd.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    pwd += i.KeyChar;
                    Console.Write("*");
                }
            }
            return pwd;
        }

        public static void WriteConfig(Configuration config, string password)
        {
            string configString = JsonConvert.SerializeObject(config, Formatting.Indented);
            string encryptedConfigString = Aes.Encrypt(configString, password);
            File.WriteAllText(absolutePath, encryptedConfigString);
        }
    }
}
