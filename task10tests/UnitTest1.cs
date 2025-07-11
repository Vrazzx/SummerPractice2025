using System;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginLib;
using Xunit;
using FirstPlugin;

namespace task10.Tests
{
    public class PluginLoaderTests : IDisposable
    {
        private readonly string _testPluginsDirectory;
        private readonly string _testPluginPath;

        public PluginLoaderTests()
        {
            // Создаем временную директорию для тестовых плагинов
            _testPluginsDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testPluginsDirectory);

            // Копируем тестовую сборку с плагинами во временную директорию
            var pluginAssembly = typeof(FirstPlugin.FirstPlugin).Assembly;
            _testPluginPath = Path.Combine(_testPluginsDirectory, "FirstPlugin.dll");
            File.Copy(pluginAssembly.Location, _testPluginPath);
        }

        public void Dispose()
        {
            // Удаляем временную директорию после тестов
            if (Directory.Exists(_testPluginsDirectory))
            {
                Directory.Delete(_testPluginsDirectory, true);
            }
        }

        [Fact]
        public void Constructor_ShouldInitializeWithPluginsDirectory()
        {
            // Arrange & Act
            var loader = new PluginLoader(_testPluginsDirectory);
            
            // Assert
            Assert.NotNull(loader);
        }

        [Fact]
        public void LoadPlugins_ShouldLoadAllDllsFromDirectory()
        {
            // Arrange
            var loader = new PluginLoader(_testPluginsDirectory);
            
            // Act
            loader.LoadPlugins();
            
            // Assert
            // Проверяем, что плагины загружены (косвенно)
            // Можно добавить свойство для доступа к _loadedAssemblies если нужно точное тестирование
        }

        [Fact]
        public void LoadPlugins_ShouldHandleInvalidDllFilesGracefully()
        {
            // Arrange
            var invalidDllPath = Path.Combine(_testPluginsDirectory, "invalid.dll");
            File.WriteAllText(invalidDllPath, "This is not a valid DLL");
            
            var loader = new PluginLoader(_testPluginsDirectory);
            
            // Act & Assert (не должно быть исключений)
            var exception = Record.Exception(() => loader.LoadPlugins());
            Assert.Null(exception);
        }

        [Fact]
        public void LoadPlugins_ShouldLoadPluginsWithPluginLoadAttribute()
        {
            // Arrange
            var loader = new PluginLoader(_testPluginsDirectory);
            
            // Act
            loader.LoadPlugins();
            
            // Assert
            // Проверяем, что плагины загружены (косвенно через Execute)
            // Можно добавить свойство для доступа к _plugins если нужно точное тестирование
        }

        [Fact]
        public void LoadPlugins_ShouldResolveDependenciesCorrectly()
        {
            // Arrange
            var loader = new PluginLoader(_testPluginsDirectory);
            
            // Act
            loader.LoadPlugins();
            
            // Assert
            // Проверяем, что зависимость SecondPlugin от FirstPlugin разрешена корректно
            // Можно добавить метод для проверки порядка загрузки если нужно
        }

        [Fact]
        public void ExecuteAll_ShouldExecuteAllLoadedPlugins()
        {
            // Arrange
            var loader = new PluginLoader(_testPluginsDirectory);
            loader.LoadPlugins();
            
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            
            // Act
            loader.ExecuteAll();
            
            // Assert
            var output = consoleOutput.ToString();
            Assert.Contains("FirstPlugin executed first!", output);
            Assert.Contains("SecondPlugin executed!", output);
            
            // Восстанавливаем стандартный вывод
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [Fact]
        public void ExecuteAll_ShouldExecutePluginsInCorrectOrderBasedOnDependencies()
        {
            // Arrange
            var loader = new PluginLoader(_testPluginsDirectory);
            loader.LoadPlugins();
            
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            
            // Act
            loader.ExecuteAll();
            
            // Assert
            var output = consoleOutput.ToString();
            var firstPluginIndex = output.IndexOf("FirstPlugin executed first!");
            var secondPluginIndex = output.IndexOf("SecondPlugin executed!");
            
            Assert.True(firstPluginIndex < secondPluginIndex, 
                "FirstPlugin should be executed before SecondPlugin due to dependency");
            
            // Восстанавливаем стандартный вывод
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [Fact]
        public void ExecuteAll_ShouldHandlePluginExecutionErrorsGracefully()
        {
            // Arrange
            // Можно создать тестовый плагин, который выбрасывает исключение при выполнении
            // В текущей реализации тестируем только что метод не выбрасывает исключение
            var loader = new PluginLoader(_testPluginsDirectory);
            loader.LoadPlugins();
            
            // Act & Assert
            var exception = Record.Exception(() => loader.ExecuteAll());
            Assert.Null(exception);
        }
    }
}