using System.Runtime.ExceptionServices;

namespace net_task;
public class NetTask
{
    private readonly object _lock = new();

    private bool _completed;
    private Exception? _exception;
    private Action? _action;
    private ExecutionContext? _context;
    public bool IsCompleted
    {
        get
        {
            lock (_lock)
            {
                return _completed;
            }
        }
    }

    public static NetTask Run(Action action)
    {
        var task = new NetTask();
        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                action();
                task.SetResult();
            }
            catch (Exception ex)
            {
                task.SetException(ex);
            }
        });
        return task;
    }

    public NetTask ContinueWith(Action action)
    {
        var task = new NetTask();
        lock(_lock)
        {
            if (_completed)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        action();
                        task.SetResult();
                    }
                    catch (Exception ex)
                    {
                        task.SetException(ex);
                    }
                });
            }
            else
            {
                _action = action;
                _context = ExecutionContext.Capture();
            }
        }
        return task;
    }

    public void Wait()
    {
        ManualResetEventSlim?  manualResetEventSlim = null;

        lock (_lock)
        {

            if (!_completed)
            {
                manualResetEventSlim = new ManualResetEventSlim();
                ContinueWith(() => manualResetEventSlim.Set());
            }
        }
        manualResetEventSlim?.Wait();

        if (_exception is not null)
        {
            ExceptionDispatchInfo.Throw(_exception);
        }
    }

    public NetTaskAwaiter GetAwaiter() => new(this);

    public static NetTask Delay(TimeSpan delay)
    {
        var task = new NetTask();

        new Timer(_ => task.SetResult()).Change(delay, Timeout.InfiniteTimeSpan);
        return task;
    }

    public void SetException(Exception ex) => CompleteTask(ex);
    public void SetResult() => CompleteTask();

    public void CompleteTask(Exception? exception = null)
    {
        lock (_lock)
        {
            if (_completed)
            {
                throw new InvalidOperationException("Task already completed");
            }
            _completed = true;
            _exception = exception;

            if(_action is not null )
            {
                if(_context is null)
                {
                    _action();
                }
                else
                {
                    ExecutionContext.Run(_context, state => ((Action?)state)?.Invoke(), _action);
                }
            }
        }
    }
}
