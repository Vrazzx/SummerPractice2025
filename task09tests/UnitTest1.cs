using System;
using System.IO;
using System.Reflection;
using Xunit;
using task07;
using FileSystemCommands;

namespace AssemblyMetadataAnalyzer.Tests
{
    public class AssemblyAnalyzerTests
    {
        private readonly string _testAssemblyPath;

        public AssemblyAnalyzerTests()
        {
            _testAssemblyPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../../../../FileSystemCommands/bin/Debug/net8.0/FileSystemCommands.dll");
        }

        [Fact]
        public void Analyze_WithInvalidPath_ShouldOutputErrorMessage()
        {
            
            var invalidPath = "nonexistent.dll";
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            
            AssemblyAnalyzer.Analyze(invalidPath);

            
            var output = consoleOutput.ToString();
            Assert.Contains("Ошибка загрузки сборки", output);
        }

        [Fact]
        public void Analyze_WithValidAssembly_ShouldOutputAssemblyName()
        {
            
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            
            AssemblyAnalyzer.Analyze(_testAssemblyPath);

            
            var output = consoleOutput.ToString();
            Assert.Contains("Анализ сборки: FileSystemCommands,", output);
        }

        [Fact]
        public void PrintTypeMetadata_ShouldOutputTypeInformation()
        {
           
            var assembly = Assembly.LoadFrom(_testAssemblyPath);
            var type = assembly.GetTypes().First();
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            
            AssemblyAnalyzer.PrintTypeMetadata(type);

            
            var output = consoleOutput.ToString();
            Assert.Contains($"Класс: {type.Name}", output);
            Assert.Contains("___________________ Конструкторы: ___________________", output);
            Assert.Contains("___________________ Свойства: ___________________", output);
            Assert.Contains("___________________ Методы: ___________________", output);
        }

        [Fact]
        public void PrintAttributes_ShouldOutputAttributeInformation()
        {
            
            var assembly = Assembly.LoadFrom(_testAssemblyPath);
            var type = assembly.GetTypes().First(t => t.GetCustomAttributes<DisplayNameAttribute>().Any());
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

           
            AssemblyAnalyzer.PrintAttributes("Test Attributes", type.GetCustomAttributes());

            
            var output = consoleOutput.ToString();
            Assert.Contains("Test Attributes", output);
            Assert.Contains("DisplayNameAttribute", output);
        }

        [Fact]
        public void PrintMembers_ShouldOutputMemberInformation()
        {
            
            var assembly = Assembly.LoadFrom(_testAssemblyPath);
            var type = assembly.GetTypes().First();
            var methods = type.GetMethods().Where(m => !m.IsSpecialName).Take(1).ToList();
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            
            AssemblyAnalyzer.PrintMembers("Test Members", methods, m => { });

            
            var output = consoleOutput.ToString();
            Assert.Contains("Test Members", output);
            Assert.Contains(methods[0].Name, output);
        }

        [Fact]
        public void PrintParameters_ShouldOutputParameterInformation()
        {
            
            var assembly = Assembly.LoadFrom(_testAssemblyPath);
            var method = assembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .First(m => m.GetParameters().Length > 0);
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            
            AssemblyAnalyzer.PrintParameters(method.GetParameters());

            
            var output = consoleOutput.ToString();
            Assert.Contains("Параметры:", output);
            Assert.Contains(method.GetParameters()[0].Name, output);
            Assert.Contains(method.GetParameters()[0].ParameterType.Name, output);
        }

        [Fact]
        public void Analyze_ShouldHandleTypesWithDisplayNameAttribute()
        {
            
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            
            AssemblyAnalyzer.Analyze(_testAssemblyPath);

            
            var output = consoleOutput.ToString();
            Assert.Contains("DisplayName:", output);
        }

        [Fact]
        public void Analyze_ShouldHandleTypesWithVersionAttribute()
        {
            
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            
            AssemblyAnalyzer.Analyze(_testAssemblyPath);

            
            var output = consoleOutput.ToString();
            Assert.Contains("Версия:", output);
        }
    }
}