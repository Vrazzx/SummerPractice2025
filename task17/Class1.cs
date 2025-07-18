namespace task17;

using System;
using System.Collections.Concurrent;
using System.Threading;

public interface ICommand
{
    void Execute();
}

public interface IExceptionHandler
{
    void Handle(Exception exception, ICommand command);
}

public class ServerThread : IDisposable
{
    private readonly Thread _thread;
    private readonly ConcurrentQueue<ICommand> _commandQueue = new ConcurrentQueue<ICommand>();
    private readonly AutoResetEvent _commandAvailable = new AutoResetEvent(false);
    private readonly IExceptionHandler _exceptionHandler;
    private volatile bool _isRunning;
    private volatile bool _softStopRequested;

    public ServerThread(IExceptionHandler exceptionHandler = null)
    {
        _exceptionHandler = exceptionHandler;
        _isRunning = true;
        _thread = new Thread(ProcessCommands) { IsBackground = true };
        _thread.Start();
    }

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
                if (_commandQueue.TryDequeue(out var command))
                {
                    ExecuteCommand(command);
                }
                else if (_softStopRequested)
                {
                    _isRunning = false;
                }
                else
                {
                    _commandAvailable.WaitOne();
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