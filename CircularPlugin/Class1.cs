namespace CircularPlugin;

using PluginLib;

[PluginLoad("CircularPluginA", Dependencies = new[] { typeof(CircularPluginB) })]
public class CircularPluginA : IPlugin
{
    public void Execute()
    {
        Console.WriteLine("CircularPluginA executed!");
    }
}

[PluginLoad("CircularPluginB", Dependencies = new[] { typeof(CircularPluginA) })]
public class CircularPluginB : IPlugin
{
    public void Execute()
    {
        Console.WriteLine("CircularPluginB executed!");
    }
}
