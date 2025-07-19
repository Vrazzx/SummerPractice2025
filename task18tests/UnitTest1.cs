namespace task18tests;
using System;
using task17;
using Xunit;
using System.Collections.Generic;
using System.Threading;
public class ServerThreadTests
{
    [Fact]
    public void Should_ExecuteRegularCommand_Immediately()
    {
        
        var server = new ServerThread();
        var executed = false;
        var command = new ActionCommand(() => executed = true);

        
        server.EnqueueCommand(command);
        Thread.Sleep(50); 

        
        Assert.True(executed);
    }

    [Fact]
    public void Should_ProcessLongRunningCommand_InMultipleSteps()
    {
      
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(null, scheduler);
        var longRunningCmd = new TestLongRunningCommand(3);

    
        server.EnqueueCommand(longRunningCmd);
        Thread.Sleep(150); 

  
        Assert.Equal(3, longRunningCmd.ExecutionCount);
        Assert.True(longRunningCmd.IsCompleted);
    }

    [Fact]
    public void Should_ProcessNewCommands_WhileRunningLongRunningOnes()
    {
      
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(null, scheduler);
        var longRunningCmd = new TestLongRunningCommand(3);
        var regularCmdExecuted = false;
        var regularCmd = new ActionCommand(() => regularCmdExecuted = true);

     
        server.EnqueueCommand(longRunningCmd);
        Thread.Sleep(30); 
        server.EnqueueCommand(regularCmd);
        Thread.Sleep(150);

        
        Assert.True(regularCmdExecuted);
        Assert.Equal(3, longRunningCmd.ExecutionCount);
    }

    [Fact]
    public void Should_CompleteAllCommands_OnSoftStop()
    {
       
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(null, scheduler);
        var cmd1 = new TestLongRunningCommand(2);
        var cmd2 = new TestLongRunningCommand(2);
        var stopCmd = new SoftStopCommand(server);

      
        server.EnqueueCommand(cmd1);
        server.EnqueueCommand(cmd2);
        server.EnqueueCommand(stopCmd);
        Thread.Sleep(150); 

        Assert.Equal(2, cmd1.ExecutionCount);
        Assert.Equal(2, cmd2.ExecutionCount);
    }

    [Fact]
    public void Should_StopImmediately_OnHardStop()
    {
        
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(null, scheduler);
        var cmd1 = new TestLongRunningCommand(10); 
        var stopCmd = new HardStopCommand(server);

    
        server.EnqueueCommand(cmd1);
        Thread.Sleep(30); 
        server.EnqueueCommand(stopCmd);
        Thread.Sleep(100); 

  
        Assert.True(cmd1.ExecutionCount < 10); 
    }

    [Fact]
    public void Should_NotBlock_WhenSchedulerHasCommands()
    {
        
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(null, scheduler);
        var cmd1 = new TestLongRunningCommand(2);
        var cmd2 = new TestLongRunningCommand(2);
        var executionOrder = new List<int>();

        cmd1.OnExecute = () => executionOrder.Add(1);
        cmd2.OnExecute = () => executionOrder.Add(2);

        
        server.EnqueueCommand(cmd1);
        Thread.Sleep(30); 
        server.EnqueueCommand(cmd2);
        Thread.Sleep(150); 

        
        Assert.Equal(4, executionOrder.Count); 
    }

    [Fact]
    public void Should_ProcessRegularCommands_WhenSchedulerIsEmpty()
    {
        
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(null, scheduler);
        var regularCmdExecuted = false;
        var regularCmd = new ActionCommand(() => regularCmdExecuted = true);

        
        server.EnqueueCommand(regularCmd);
        Thread.Sleep(50); 

        
        Assert.True(regularCmdExecuted);
    }

}


public class ActionCommand : ICommand
{
    private readonly Action _action;

    public ActionCommand(Action action)
    {
        _action = action;
    }

    public void Execute()
    {
        _action();
    }
}

public class TestLongRunningCommand : ILongRunningCommand
{
    private int _remainingExecutions;
    public int ExecutionCount { get; private set; }
    public Action OnExecute { get; set; }

    public TestLongRunningCommand(int requiredExecutions)
    {
        _remainingExecutions = requiredExecutions;
    }

    public bool IsCompleted => _remainingExecutions <= 0;

    public void Execute()
    {
        ExecutionCount++;
        _remainingExecutions--;
        OnExecute?.Invoke();
        Thread.Sleep(20);
    }
}

public class FailingLongRunningCommand : ILongRunningCommand
{
    private int _remainingExecutions;
    public int ExecutionCount { get; private set; }

    public FailingLongRunningCommand(int requiredExecutions)
    {
        _remainingExecutions = requiredExecutions;
    }

    public bool IsCompleted => _remainingExecutions <= 0;

    public void Execute()
    {
        ExecutionCount++;
        _remainingExecutions--;
        throw new Exception("Simulated error");
    }
}

public class TestExceptionHandler : IExceptionHandler
{
    public List<(Exception, ICommand)> HandledExceptions { get; } = new List<(Exception, ICommand)>();

    public void Handle(Exception exception, ICommand command)
    {
        HandledExceptions.Add((exception, command));
    }
}
