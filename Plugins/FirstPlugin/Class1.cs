using PluginLib;
namespace FirstPlugin;

[PluginLoad("FirstPlugin")]
public class FirstPlugin : IPlugin
{
    public void Execute()
    {
        Console.WriteLine("FirstPlugin executed first!");
    }
}

[PluginLoad("SecondPlugin", Dependencies = new[] { typeof(FirstPlugin) })]
public class SecondPlugin : IPlugin
{
    public void Execute()
    {
        Console.WriteLine("SecondPlugin executed!");
    }
}