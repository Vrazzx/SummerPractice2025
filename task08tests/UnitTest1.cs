namespace task08tests;
using FileSystemCommands;

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
