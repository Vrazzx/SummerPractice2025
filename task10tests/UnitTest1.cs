namespace task10tests;

using Xunit;
using task10;
using System.IO;
using System.Reflection;

public class PluginLoaderTests
{
    [Fact]
    public void Should_Load_Plugins_From_Directory()
    {
        var loader = new PluginLoader();
        string testPluginsPath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "TestPlugins");

        loader.LoadPluginsFromDirectory(testPluginsPath);
        
        // Проверки
    }

    [Fact]
    public void Should_Resolve_Dependencies_Correctly()
    {
        // Тест на правильный порядок загрузки
    }

    [Fact]
    public void Should_Throw_On_Circular_Dependencies()
    {
        // Тест на циклические зависимости
    }
}