using PluginLib;

namespace Plugin;

using task10;

[PluginLoad("SamplePlugin")]
public class SamplePlugin : IPluginCommand
{
    public void Execute()
    {
        Console.WriteLine("SamplePlugin executed! Hello from Plugin1!");
    }
}