using System;
using System.IO;
using System.Linq;
using Xunit;
using PluginLib;
using System.Collections.Generic;
using System.Reflection;

namespace task10.Tests
{
    public class PluginLoaderTests
    {
        private readonly string _testPluginsPath = Path.Combine(Directory.GetCurrentDirectory(), "../../../../Plugins");

        [Fact]
        public void LoadPlugins_ShouldLoadAllPluginsFromDirectory()
        {

            var pluginLoader = new PluginLoader(_testPluginsPath);


            pluginLoader.LoadPlugins();


            var pluginsField = typeof(PluginLoader).GetField("_plugins", BindingFlags.NonPublic | BindingFlags.Instance);
            var pluginsDict = (Dictionary<Type, IPlugin>)pluginsField.GetValue(pluginLoader);


            Assert.Equal(5, pluginsDict.Count);
            Assert.Contains(pluginsDict.Keys, t => t.Name == "FirstPlugin");
            Assert.Contains(pluginsDict.Keys, t => t.Name == "SubFirstPlugin");
            Assert.Contains(pluginsDict.Keys, t => t.Name == "SecondPlugin");
            Assert.Contains(pluginsDict.Keys, t => t.Name == "SubSecondPlugin");
            Assert.Contains(pluginsDict.Keys, t => t.Name == "Sub2SecondPlugin");
        }

        [Fact]
        public void LoadPlugins_ShouldResolveDependenciesCorrectly()
        {

            var pluginLoader = new PluginLoader(_testPluginsPath);


            pluginLoader.LoadPlugins();


            var pluginsField = typeof(PluginLoader).GetField("_plugins", BindingFlags.NonPublic | BindingFlags.Instance);
            var pluginsDict = (Dictionary<Type, IPlugin>)pluginsField.GetValue(pluginLoader);


            var loadedOrder = pluginsDict.Keys.ToList();


            var firstPluginIndex = loadedOrder.FindIndex(t => t.Name == "FirstPlugin");
            var subFirstPluginIndex = loadedOrder.FindIndex(t => t.Name == "SubFirstPlugin");
            var secondPluginIndex = loadedOrder.FindIndex(t => t.Name == "SecondPlugin");
            var subSecondPluginIndex = loadedOrder.FindIndex(t => t.Name == "SubSecondPlugin");
            var sub2SecondPluginIndex = loadedOrder.FindIndex(t => t.Name == "Sub2SecondPlugin");


            Assert.True(subFirstPluginIndex < secondPluginIndex);


            Assert.True(secondPluginIndex < subSecondPluginIndex);


            Assert.True(firstPluginIndex < sub2SecondPluginIndex);
        }

        [Fact]
        public void ExecuteAll_ShouldExecuteAllPluginsWithoutErrors()
        {

            var pluginLoader = new PluginLoader(_testPluginsPath);
            pluginLoader.LoadPlugins();
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);


            pluginLoader.ExecuteAll();
            var output = consoleOutput.ToString();


            Assert.Contains("FirstPlugin is running!", output);
            Assert.Contains("SubFirstPlugin is running!", output);
            Assert.Contains("SecondPlugin is running! (Depends on FirstPlugin)", output);
            Assert.Contains("SubSecondPlugin is running!", output);
            Assert.Contains("Sub2SecondPlugin is running!", output);
        }

        [Fact]
        public void LoadPlugins_ShouldThrowOnCircularDependencies()
        {

            var circularPluginsPath = Path.Combine(Directory.GetCurrentDirectory(), "../../../../CircularPlugin");

            var pluginLoader = new PluginLoader(circularPluginsPath);


            var ex = Assert.Throws<InvalidOperationException>(() => pluginLoader.LoadPlugins());
            Assert.Contains("Cyclic dependency detected", ex.Message);
        }

        [Fact]
        public void LoadPlugins_ShouldHandleEmptyDirectory()
        {
            // Arrange
            var emptyDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(emptyDir);

            var pluginLoader = new PluginLoader(emptyDir);
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            pluginLoader.LoadPlugins();

            // Assert
            var output = consoleOutput.ToString();
            Assert.DoesNotContain("Failed", output); // Не должно быть ошибок

            var pluginsField = typeof(PluginLoader).GetField("_plugins", BindingFlags.NonPublic | BindingFlags.Instance);
            var plugins = (Dictionary<Type, IPlugin>)pluginsField.GetValue(pluginLoader);
            Assert.Empty(plugins);
        }

    }
}
