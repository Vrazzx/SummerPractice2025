using CommandLib;
using System.Reflection;

namespace CommandRunner {
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                CommandRunnerApp();
            }
        }
        public static void CommandRunnerApp()
        {
            System.Console.WriteLine("Запущено консольное приложение");
            System.Console.WriteLine("Доступные комманды: ");
            System.Console.WriteLine("1. DirectorySizeCommand");
            System.Console.WriteLine("2. FindFilesCommand");
            System.Console.WriteLine("Выберете команду (1/2): ");

            var choice = System.Console.ReadLine();
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileSystemCommands.dll");

            if (!File.Exists(dllPath)) {
                Console.WriteLine($"Файл {dllPath} не найден.");
                return;
            }

            var assembly = Assembly.LoadFrom(dllPath);
            ICommand command = null;

            try
            {
                switch (choice)
                {
                    case "1":
                        System.Console.WriteLine("Введите путь к директории: ");
                        var dirPath = Console.ReadLine();
                        command = (ICommand)Activator.CreateInstance(assembly.GetType("FileSystemCommands.DirectorySizeCommand"), dirPath);
                        break;
                    case "2":
                        System.Console.WriteLine("Введите путь к директории: ");
                        var searchDir = Console.ReadLine();
                        System.Console.WriteLine("Введите искомое расширение: ");
                        var pattern = Console.ReadLine();
                        command = (ICommand)Activator.CreateInstance(assembly.GetType("FileSystemCommands.FindFilesCommand"), searchDir, pattern);
                        break;
                    default:
                        System.Console.WriteLine("Инвалид чойс");
                        break;
                }

                command?.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

        }
    }
}
