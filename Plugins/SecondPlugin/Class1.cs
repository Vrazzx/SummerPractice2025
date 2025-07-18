using PluginLib;

namespace SecondPlugin;

using PluginLib;

[PluginLoad("SecondPlugin", Dependencies = new[] { typeof(FirstPlugin.SubFirstPlugin) })]
public class SecondPlugin : IPlugin
{
    public void Execute()
    {
        Console.WriteLine("SecondPlugin is running! (Depends on FirstPlugin)");
    }
}

[PluginLoad("SubSecondPlugin", Dependencies = new[] { typeof(SecondPlugin) })]
public class SubSecondPlugin : IPlugin
{
    public void Execute()
    {
        Console.WriteLine("SubSecondPlugin is running!");
    }
}
[PluginLoad("Sub2SecondPlugin", Dependencies = new[] { typeof(FirstPlugin.FirstPlugin) })]
public class Sub2SecondPlugin : IPlugin
{
    public void Execute()
    {
        Console.WriteLine("Sub2SecondPlugin is running!");
    }
}
