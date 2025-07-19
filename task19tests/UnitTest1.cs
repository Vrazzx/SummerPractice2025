namespace task19tests;

using Xunit;
using System.Threading;
using System.Linq;
using task17;
public class LongRunningCommandsTests
{
    [Fact]
    public void Should_ExecuteLongRunningCommand_MultipleTimes()
    {
        
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(null, scheduler);
        var command = new TestCommand(1);
        
        
        server.EnqueueCommand(command);
        Thread.Sleep(350); 
        
        
        Assert.True(command.IsCompleted);
    }

   [Fact]
    public void Should_InterleaveMultipleLongRunningCommands()
    {
        
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(null, scheduler);
        var commands = Enumerable.Range(1, 5)
            .Select(id => new TestCommand(id))
            .ToList();
        
        
        foreach (var cmd in commands)
        {
            server.EnqueueCommand(cmd);
        }
        
        
        bool allCompleted = SpinWait.SpinUntil(() => 
            commands.All(c => c.IsCompleted), 
            TimeSpan.FromSeconds(3));
        
      
        Assert.True(allCompleted, "Не все команды завершились за отведённое время");
        Assert.All(commands, cmd => 
        {
            Assert.True(cmd.IsCompleted, $"Команда {cmd.Id} не завершена");
            Assert.Equal(3, cmd.Counter);
        });
    }

    [Fact]
    public void Should_StopWithHardStopCommand()
    {
        
        var server = new ServerThread();
        var longCommand = new TestCommand(1, 10);
        var stopCommand = new HardStopCommand(server);
        
        
        server.EnqueueCommand(longCommand);
        Thread.Sleep(150);
        server.EnqueueCommand(stopCommand);
        Thread.Sleep(300);
        
        
        Assert.True(longCommand.Counter < 10);
    }

    [Fact]
    public void Should_CancelLongRunningCommand()
    {
        
        var server = new ServerThread();
        var command = new CancellableCommand(1);
        
        
        server.EnqueueCommand(command);
        Thread.Sleep(200);
        command.Cancel();
        var counterAfterCancel = command.Counter;
        Thread.Sleep(300);
        
        
        Assert.True(command.IsCompleted);
        Assert.Equal(counterAfterCancel, command.Counter);
    }
}