using CommandLib;
using System.IO;

namespace FileSystemCommands;

public class DirectorySizeCommand : ICommand
{
    private readonly string _directoryPath;

    public DirectorySizeCommand(string directoryPath)
    {
        _directoryPath = directoryPath;
    }

    public void Execute()
    {
        if (!Directory.Exists(_directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory '{_directoryPath}' not found.");
        }
        long size = CalculateDirectorySize(_directoryPath);
        Console.WriteLine($"Directory size: {size} bytes");
    }

    private long CalculateDirectorySize(string directory)
    {
        long size = 0;
        var files = Directory.GetFiles(directory);
        foreach (var file in files)
        {
            size += new FileInfo(file).Length;
        }

        var directories = Directory.GetDirectories(directory);
        foreach (var dir in directories)
        {
            size += CalculateDirectorySize(dir);
        }
        return size;
    }
}

public class FindFilesCommand : ICommand
{
    private readonly string _directoryPath;
    private readonly string _searchPattern;

    public FindFilesCommand(string directoryPath, string searchPattern)
    {
        _directoryPath = directoryPath;
        _searchPattern = searchPattern;
    }

    public void Execute()
    {
        if (!Directory.Exists(_directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory '{_directoryPath}' not found.");
        }

        var files = Directory.GetFiles(_directoryPath, _searchPattern);
        System.Console.WriteLine($"Found {files.Length} files matching '{_searchPattern}'");
        foreach (var file in files)
        {
            System.Console.WriteLine(file);
        }
    }
}
