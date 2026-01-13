namespace task08tests;
using FileSystemCommands;
using Xunit;
using Moq;
using CommandLib;
using System.Reflection;
using CommandRunner;
public class FileSystemCommandsTests
{
    [Fact]
    public void DirectorySizeCommand_ShouldCalculateSize()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "test1.txt"), "Hello");
        File.WriteAllText(Path.Combine(testDir, "test2.txt"), "World");
        var exceptedSize = "Hello".Length + "World".Length;

        var command = new DirectorySizeCommand(testDir);
        command.Execute(); // Проверяем, что не возникает исключений
        var size = command.CalculateDirectorySize(testDir);

        Assert.Equal(exceptedSize, size);


        Directory.Delete(testDir, true);
    }

    [Fact]
    public void FindFilesCommand_ShouldFindMatchingFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "TestDir");
        Directory.CreateDirectory(testDir);
        File.WriteAllText(Path.Combine(testDir, "file1.txt"), "Text");
        File.WriteAllText(Path.Combine(testDir, "file2.log"), "Log");


        var command = new FindFilesCommand(testDir, "*.txt");
        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput); // Перехватываем вывод в консоль


        command.Execute();


        var output = consoleOutput.ToString();


        Assert.Contains("Found 1 files matching '*.txt'", output);


        Assert.Contains("file1.txt", output);


        Assert.DoesNotContain("file2.log", output);


        Directory.Delete(testDir, true);
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
    }

    [Fact]
    public void DirectorySizeCommand_ShouldReturnZeroForEmptyDirectory()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "EmptyDir");
        Directory.CreateDirectory(testDir);


        var exceptedSize = 0;
        var command = new DirectorySizeCommand(testDir);
        var size = command.CalculateDirectorySize(testDir);

        command.Execute();


        Assert.Equal(exceptedSize, size);
        Directory.Delete(testDir, true);
    }
    [Fact]
    public void FindFilesCommand_NoEx_WhenNoFilesMatch()
    {

        var nonExistentDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var command = new FindFilesCommand(nonExistentDir, "*.txt");


        var ex = Assert.Throws<DirectoryNotFoundException>(() => command.Execute());


        Assert.Equal($"Directory '{nonExistentDir}' not found.", ex.Message);
    }
}

    public class CommandRunnerTests : IDisposable
    {
        private readonly StringWriter _stringWriter;
        private readonly StringReader _stringReader;
        private readonly string _testDllPath;
        private readonly string _testDirPath;
        private readonly string _testExtension;

        public CommandRunnerTests()
        {
            
            _stringWriter = new StringWriter();
            _stringReader = new StringReader("");
            Console.SetOut(_stringWriter);
            Console.SetIn(_stringReader);

            
            _testDllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileSystemCommands.dll");
            _testDirPath = Path.GetTempPath();
            _testExtension = "*.txt";
        }

        public void Dispose()
        {
            _stringWriter.Dispose();
            _stringReader.Dispose();
            
            
            var standardOutput = new StreamWriter(Console.OpenStandardOutput());
            Console.SetOut(standardOutput);
            
            var standardInput = new StreamReader(Console.OpenStandardInput());
            Console.SetIn(standardInput);
        }

        [Fact]
        public void Main_WithNoArgs_StartsCommandRunnerApp()
        {
            
            var args = Array.Empty<string>();
            var expectedOutput = "Запущено консольное приложение";

            
            Program.Main(args);
            var output = _stringWriter.ToString();

            
            Assert.Contains(expectedOutput, output);
        }

    


        [Fact]
        public void CommandRunnerApp_WithInvalidChoice_DisplaysErrorMessage()
        {
            
            SetConsoleInput("3"); 

            
            Program.CommandRunnerApp();
            var output = _stringWriter.ToString();

            
            Assert.Contains("Инвалид чойс", output);
        }

        [Fact]
        public void CommandRunnerApp_WithMissingDll_DisplaysErrorMessage()
        {
            
            var tempPath = Path.GetTempFileName();
            File.Delete(tempPath); 
            SetConsoleInput("1\n" + _testDirPath);

            try
            {
                
                if (File.Exists(_testDllPath))
                {
                    File.Move(_testDllPath, _testDllPath + ".bak");
                }

                
                Program.CommandRunnerApp();
                var output = _stringWriter.ToString();

                
                Assert.Contains($"Файл {_testDllPath} не найден.", output);
            }
            finally
            {
                
                if (File.Exists(_testDllPath + ".bak"))
                {
                    File.Move(_testDllPath + ".bak", _testDllPath);
                }
            }
        }

        [Fact]
        public void CommandRunnerApp_WithChoice1_CreatesDirectorySizeCommand()
        {
            
            SetConsoleInput("1\n" + _testDirPath);

            
            Program.CommandRunnerApp();
            var output = _stringWriter.ToString();

            
            Assert.Contains("Введите путь к директории:", output);
           
        }

        [Fact]
        public void CommandRunnerApp_WithChoice2_CreatesFindFilesCommand()
        {
         
            SetConsoleInput("2\n" + _testDirPath + "\n" + _testExtension);

          
            Program.CommandRunnerApp();
            var output = _stringWriter.ToString();

            
            Assert.Contains("Введите путь к директории:", output);
            Assert.Contains("Введите искомое расширение:", output);
            
        }

        [Fact]
        public void CommandRunnerApp_WithInvalidDirectoryPath_DisplaysErrorMessage()
        {
          
            var invalidPath = "Z:\\nonexistent\\path\\";
            SetConsoleInput("1\n" + invalidPath);

            
            Program.CommandRunnerApp();
            var output = _stringWriter.ToString();

            
            Assert.Contains("Ошибка:", output);
        }

        private void SetConsoleInput(string input)
        {
            _stringReader.Dispose();
            Console.SetIn(new StringReader(input));
        }
    }

    
    public class TestCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("TestCommand executed");
        }
    }



