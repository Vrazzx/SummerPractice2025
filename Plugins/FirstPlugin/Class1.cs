using PluginLib;

namespace FirstPlugin;

using PluginLib;

[PluginLoad("FirstPlugin")]
public class FirstPlugin : IPlugin
{
    public void Execute()
    {
        Console.WriteLine("FirstPlugin is running!");
    }
}

[PluginLoad("SubFirstPlugin")]
public class SubFirstPlugin : IPlugin
{
    public void Execute()
    {
        Console.WriteLine("SubFirstPlugin is running!");
    }
}
