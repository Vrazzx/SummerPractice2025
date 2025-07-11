using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginLib;


namespace task10;
public class PluginLoader
{
    private readonly string _pluginsDirectory;
    private readonly List<Assembly> _loadedAssemblies = new List<Assembly>();
    private readonly Dictionary<Type, IPlugin> _plugins = new Dictionary<Type, IPlugin>();

    public PluginLoader(string pluginsDirectory)
    {
        _pluginsDirectory = pluginsDirectory;
    }

    public void LoadPlugins()
    {
        // Загрузка всех DLL из указанной директории
        var pluginFiles = Directory.GetFiles(_pluginsDirectory, "*.dll");

        foreach (var file in pluginFiles)
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);
                _loadedAssemblies.Add(assembly);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load assembly {file}: {ex.Message}");
            }
        }

        // Находим все типы с атрибутом PluginLoad
        var pluginTypes = new List<Type>();
        foreach (var assembly in _loadedAssemblies)
        {
            try
            {
                var types = assembly.GetTypes()
                    .Where(t => t.GetCustomAttribute<PluginLoadAttribute>() != null &&
                                typeof(IPlugin).IsAssignableFrom(t));
                pluginTypes.AddRange(types);
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"Failed to load types from {assembly.FullName}: {ex.Message}");
            }
        }

        // Создаем граф зависимостей
        var graph = new DependencyGraph<Type>();
        foreach (var type in pluginTypes)
        {
            var attr = type.GetCustomAttribute<PluginLoadAttribute>();
            graph.AddNode(type);

            foreach (var dependency in attr.Dependencies)
            {
                graph.AddEdge(dependency, type);
            }
        }

        // Получаем порядок загрузки с учетом зависимостей
        var loadOrder = graph.TopologicalSort();

        // Создаем экземпляры плагинов в правильном порядке
        foreach (var type in loadOrder)
        {
            try
            {
                var plugin = (IPlugin)Activator.CreateInstance(type);
                _plugins.Add(type, plugin);
                Console.WriteLine($"Loaded plugin: {type.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create instance of {type.Name}: {ex.Message}");
            }
        }
    }

    public void ExecuteAll()
    {
        foreach (var plugin in _plugins.Values)
        {
            try
            {
                plugin.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing plugin {plugin.GetType().Name}: {ex.Message}");
            }
        }
    }
}