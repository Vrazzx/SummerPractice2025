using PluginLib;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace task10;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PluginLoadAttribute : Attribute
{
    public string Name { get; }
    public string[] Dependencies { get; }

    public PluginLoadAttribute(string name, params string[] dependencies)
    {
        Name = name;
        Dependencies = dependencies ?? Array.Empty<string>();
    }
}
public class PluginLoader
{
    private readonly Dictionary<string, IPluginCommand> _plugins = new();
    private readonly List<string> _loadedPluginNames = new();

    public void LoadPluginsFromDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
        }

        var dllFiles = Directory.GetFiles(directoryPath, "*.dll");
        var assemblies = new List<Assembly>();

        // Загрузка всех сборок
        foreach (var dllFile in dllFiles)
        {
            try
            {
                var assembly = Assembly.LoadFrom(dllFile);
                assemblies.Add(assembly);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load assembly {dllFile}: {ex.Message}");
            }
        }

        // Сбор информации о всех плагинах
        var pluginTypes = new List<(Type Type, PluginLoadAttribute Attr)>();
        foreach (var assembly in assemblies)
        {
            try
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<PluginLoadAttribute>();
                    if (attr != null && typeof(IPluginCommand).IsAssignableFrom(type))
                    {
                        pluginTypes.Add((type, attr));
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"Failed to get types from assembly {assembly.FullName}: {ex.Message}");
            }
        }

        // Сортировка плагинов с учетом зависимостей (топологическая сортировка)
        var sortedPlugins = TopologicalSort(pluginTypes);

        // Создание экземпляров плагинов
        foreach (var (type, attr) in sortedPlugins)
        {
            try
            {
                if (_plugins.ContainsKey(attr.Name))
                {
                    Console.WriteLine($"Plugin {attr.Name} is already loaded. Skipping.");
                    continue;
                }

                var plugin = (IPluginCommand)Activator.CreateInstance(type);
                _plugins.Add(attr.Name, plugin);
                _loadedPluginNames.Add(attr.Name);
                Console.WriteLine($"Loaded plugin: {attr.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create instance of plugin {attr.Name}: {ex.Message}");
            }
        }
    }

    public void ExecuteAll()
    {
        foreach (var pluginName in _loadedPluginNames)
        {
            if (_plugins.TryGetValue(pluginName, out var plugin))
            {
                try
                {
                    Console.WriteLine($"Executing plugin: {pluginName}");
                    plugin.Execute();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing plugin {pluginName}: {ex.Message}");
                }
            }
        }
    }

    private List<(Type Type, PluginLoadAttribute Attr)> TopologicalSort(
        List<(Type Type, PluginLoadAttribute Attr)> plugins)
    {
        var result = new List<(Type, PluginLoadAttribute)>();
        var visited = new HashSet<string>();
        var tempMark = new HashSet<string>();

        var pluginDict = plugins.ToDictionary(x => x.Attr.Name, x => x);

        foreach (var plugin in plugins)
        {
            if (!visited.Contains(plugin.Attr.Name))
            {
                Visit(plugin.Attr.Name, pluginDict, visited, tempMark, result);
            }
        }

        return result;
    }
    
        private void Visit(
        string pluginName,
        Dictionary<string, (Type Type, PluginLoadAttribute Attr)> pluginDict,
        HashSet<string> visited,
        HashSet<string> tempMark,
        List<(Type, PluginLoadAttribute)> result)
    {
        if (tempMark.Contains(pluginName))
        {
            throw new InvalidOperationException($"Circular dependency detected involving plugin {pluginName}");
        }

        if (visited.Contains(pluginName))
        {
            return;
        }

        if (!pluginDict.TryGetValue(pluginName, out var plugin))
        {
            throw new KeyNotFoundException($"Plugin {pluginName} not found in plugin dictionary");
        }

        tempMark.Add(pluginName);

        foreach (var dependency in plugin.Attr.Dependencies)
        {
            Visit(dependency, pluginDict, visited, tempMark, result);
        }

        tempMark.Remove(pluginName);
        visited.Add(pluginName);
        result.Add((plugin.Type, plugin.Attr));
    }
}