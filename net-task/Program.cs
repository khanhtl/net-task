// See https://aka.ms/new-console-template for more information
using net_task;

Console.WriteLine($"Start thread id: {Environment.CurrentManagedThreadId}");

NetTask.Run(() => Console.WriteLine($"First NetTask thread id: {Environment.CurrentManagedThreadId}"))
    .Wait();

NetTask.Delay(TimeSpan.FromSeconds(1))
    .Wait();

Console.WriteLine($"Second thread id: {Environment.CurrentManagedThreadId}");

NetTask.Delay(TimeSpan.FromSeconds(1))
    .Wait();

NetTask.Run(() => Console.WriteLine($"Third NetTask thread id: {Environment.CurrentManagedThreadId}"))
    .Wait();

