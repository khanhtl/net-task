// See https://aka.ms/new-console-template for more information
using net_task;

Console.WriteLine($"Start thread id: {Environment.CurrentManagedThreadId}");

await NetTask.Run(() => Console.WriteLine($"First NetTask thread id: {Environment.CurrentManagedThreadId}"));

await NetTask.Delay(TimeSpan.FromSeconds(1));

Console.WriteLine($"Second thread id: {Environment.CurrentManagedThreadId}");

await NetTask.Delay(TimeSpan.FromSeconds(1));

await NetTask.Run(() => Console.WriteLine($"Third NetTask thread id: {Environment.CurrentManagedThreadId}"));

