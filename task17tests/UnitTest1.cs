namespace task17tests;

using System;
using System.Threading;
using Xunit;
using task17;
public class ServerThreadTests
{
    [Fact]
    public void TestHardStop_ImmediatelyStopsThread()
    {
        
        var server = new ServerThread();
        bool commandExecuted = false;
        
        server.EnqueueCommand(new TestCommand(() => commandExecuted = true));
        server.EnqueueCommand(new HardStopCommand(server));
        server.EnqueueCommand(new TestCommand(() => throw new Exception("This should not execute")));

        
        Thread.Sleep(100); 
        
        
        Assert.True(commandExecuted);
        
    }

    [Fact]
    public void TestSoftStop_StopsAfterQueueIsEmpty()
    {
        
        var server = new ServerThread();
        bool firstCommandExecuted = false;
        bool secondCommandExecuted = false;
        
        server.EnqueueCommand(new TestCommand(() => firstCommandExecuted = true));
        server.EnqueueCommand(new SoftStopCommand(server));
        server.EnqueueCommand(new TestCommand(() => secondCommandExecuted = true));

       
        Thread.Sleep(100); 
        
        
        Assert.True(firstCommandExecuted);
        Assert.True(secondCommandExecuted);
    }

    [Fact]
    public void TestHardStopFromAnotherThread_ThrowsException()
    {
        
        var server = new ServerThread();
        var hardStop = new HardStopCommand(server);
        
        
        Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
    }

    [Fact]
    public void TestSoftStopFromAnotherThread_ThrowsException()
    {
        
        var server = new ServerThread();
        var softStop = new SoftStopCommand(server);
        
        
        Assert.Throws<InvalidOperationException>(() => softStop.Execute());
    }

    private class TestCommand : ICommand
    {
        private readonly Action _action;

        public TestCommand(Action action)
        {
            _action = action;
        }

        public void Execute()
        {
            _action?.Invoke();
        }
    }
}