using System.Runtime.CompilerServices;

namespace net_task;
public readonly struct NetTaskAwaiter : INotifyCompletion
{
    private readonly NetTask _task;
    internal NetTaskAwaiter(NetTask task) => _task = task;
    public bool IsCompleted => _task.IsCompleted;
    public void OnCompleted(Action continuation)
    {
        _task.ContinueWith(continuation);
    }

    public NetTaskAwaiter GetAwaiter() => this;

    public void GetResult() => _task.Wait();
}
