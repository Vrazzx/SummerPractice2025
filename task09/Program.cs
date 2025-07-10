using System;
using System.Reflection;
using task07;
using FileSystemCommands;

namespace AssemblyMetadataAnalyzer
{
    public class Program
    {
        static void Main(string[] args)
        {
            string assemblyPath = args.Length > 0 ? args[0] : "Ошибка загрузки сборки";
            AssemblyAnalyzer.Analyze(assemblyPath);
        }
    }

    public static class AssemblyAnalyzer
    {
        public static void Analyze(string assemblyPath)
        {
            try
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                Console.WriteLine($"Анализ сборки: {assembly.FullName}");

                foreach (var type in assembly.GetTypes())
                {
                    PrintTypeMetadata(type);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки сборки: {ex.Message} :(");
            }
        }

        public static void PrintTypeMetadata(Type type)
        {
            Console.WriteLine($"\n============= Класс: {type.Name} =============");

            PrintAttributes("Атрибуты класса", type.GetCustomAttributes());

            PrintMembers("\n___________________ Конструкторы: ___________________", 
                         type.GetConstructors(), 
                         ctor => PrintParameters(ctor.GetParameters()));

            PrintMembers("\n___________________ Свойства: ___________________",
                        type.GetProperties(),
                        prop => Console.WriteLine($"Тип: {prop.PropertyType.Name}"));

            PrintMembers("\n___________________ Методы: ___________________",
                        type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                            .Where(m => !m.IsSpecialName),
                        method => 
                        {
                            Console.WriteLine($"Возвращаемый тип: {method.ReturnType.Name}");
                            PrintParameters(method.GetParameters());
                        });
        }

        public static void PrintAttributes(string title, System.Collections.Generic.IEnumerable<Attribute> attributes)
        {
            Console.WriteLine(title);
            foreach (var attr in attributes)
            {
                Console.WriteLine($"- {attr.GetType().Name}");
                
                switch (attr)
                {
                    case DisplayNameAttribute displayNameAttr:
                        Console.WriteLine($"  DisplayName: {displayNameAttr.DisplayName}");
                        break;
                    case VersionAttribute versionAttr:
                        Console.WriteLine($"  Версия: {versionAttr.Major}.{versionAttr.Minor}");
                        break;
                }
            }
        }

        public static void PrintMembers<T>(string title, IEnumerable<T> members, Action<T> printDetails) where T : MemberInfo
        {
            Console.WriteLine(title);
            foreach (var member in members)
            {
                Console.WriteLine($"- {member.Name}");
                printDetails(member);

                var displayNameAttr = member.GetCustomAttribute<DisplayNameAttribute>();
                if (displayNameAttr != null)
                {
                    Console.WriteLine($"  DisplayName: {displayNameAttr.DisplayName}");
                }
            }
        }

        public static void PrintParameters(ParameterInfo[] parameters)
        {
            if (parameters.Length == 0) return;

            Console.WriteLine("  Параметры:");
            foreach (var param in parameters)
            {
                Console.WriteLine($"    {param.ParameterType.Name} {param.Name}");
            }
        }
    }
}
