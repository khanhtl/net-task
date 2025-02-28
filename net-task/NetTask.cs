namespace net_task;
internal class NetTask
{
    private readonly object _lock = new();

    private bool _completed;
    private Exception? _exception;
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
        try
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
        catch (Exception ex)
        {

        }
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
        }
    }
}
