namespace task17;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

public interface ICommand
{
    void Execute();
}

public interface IExceptionHandler
{
    void Handle(Exception exception, ICommand command);
}

public interface IScheduler
{
    bool HasCommand();
    ICommand Select();
    void Add(ICommand cmd);
}

public class RoundRobinScheduler : IScheduler
{
    private readonly Queue<ICommand> _commands = new Queue<ICommand>();
    private readonly object _lock = new object();

    public bool HasCommand()
    {
        lock (_lock)
        {
            return _commands.Count > 0;
        }
    }
    public int CommandCount 
    {
        get { lock (_lock) return _commands.Count; }
    }
    public ICommand Select()
    {
        lock (_lock)
        {
            if (_commands.Count == 0)
                return null;
            
            return _commands.Dequeue();
        }
    }

    public void Add(ICommand cmd)
    {
        if (cmd == null) throw new ArgumentNullException(nameof(cmd));
        
        lock (_lock)
        {
            _commands.Enqueue(cmd);
        }
    }
}

public class ServerThread : IDisposable
{
    private readonly Thread _thread;
    private readonly ConcurrentQueue<ICommand> _commandQueue = new ConcurrentQueue<ICommand>();
    private readonly AutoResetEvent _commandAvailable = new AutoResetEvent(false);
    private readonly IExceptionHandler _exceptionHandler;
    private readonly IScheduler _scheduler;
    private volatile bool _isRunning;
    private volatile bool _softStopRequested;

    public ServerThread(IExceptionHandler exceptionHandler = null, IScheduler scheduler = null)
    {
        _exceptionHandler = exceptionHandler;
        _scheduler = scheduler ?? new RoundRobinScheduler();
        _isRunning = true;
        _thread = new Thread(ProcessCommands) { IsBackground = true };
        _thread.Start();
    }
    public int GetQueueCount() => _commandQueue.Count;
    public void EnqueueCommand(ICommand command)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));
        
        _commandQueue.Enqueue(command);
        _commandAvailable.Set();
    }

    private void ProcessCommands()
    {
        try
        {
            while (_isRunning)
            {
                
                if (_scheduler.HasCommand())
                {
                    var command = _scheduler.Select();
                    if (command != null)
                    {
                        ExecuteCommand(command);
                        continue;
                    }
                }

                
                if (_commandQueue.TryDequeue(out var newCommand))
                {
                    ExecuteCommand(newCommand);
                }
                else if (_softStopRequested)
                {
                    _isRunning = false;
                }
                else
                {
                    
                    if (!_scheduler.HasCommand())
                    {
                        _commandAvailable.WaitOne();
                    }
                    else
                    {
                        
                        Thread.Yield();
                    }
                }
            }
        }
        finally
        {
            while (_commandQueue.TryDequeue(out _)) { }
        }
    }

    private void ExecuteCommand(ICommand command)
    {
        try
        {
            command.Execute();
            
            
            if (command is ILongRunningCommand longRunningCommand && longRunningCommand.IsCompleted == false)
            {
                _scheduler.Add(command);
            }
        }
        catch (Exception ex)
        {
            _exceptionHandler?.Handle(ex, command);
        }
    }

    public void RequestSoftStop()
    {
        if (Thread.CurrentThread != _thread)
        {
            throw new InvalidOperationException("SoftStop can only be executed in the server thread.");
        }
        _softStopRequested = true;
        _commandAvailable.Set();
    }

    public void RequestHardStop()
    {
        if (Thread.CurrentThread != _thread)
        {
            throw new InvalidOperationException("HardStop can only be executed in the server thread.");
        }
        _isRunning = false;
    }

    public void Dispose()
    {
        _isRunning = false;
        _commandAvailable.Set();
        _thread.Join();
        _commandAvailable.Dispose();
    }
}

public interface ILongRunningCommand : ICommand
{
    bool IsCompleted { get; }
}

public class HardStopCommand : ICommand
{
    private readonly ServerThread _serverThread;

    public HardStopCommand(ServerThread serverThread)
    {
        _serverThread = serverThread ?? throw new ArgumentNullException(nameof(serverThread));
    }

    public void Execute()
    {
        _serverThread.RequestHardStop();
    }
}

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _serverThread;

    public SoftStopCommand(ServerThread serverThread)
    {
        _serverThread = serverThread ?? throw new ArgumentNullException(nameof(serverThread));
    }

    public void Execute()
    {
        _serverThread.RequestSoftStop();
    }
}

