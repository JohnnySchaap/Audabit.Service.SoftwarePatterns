namespace Audabit.Service.SoftwarePatterns.App.Patterns._6_Concurrency;

public interface IConcurrencyPatternsService
{
    Task ThreadPoolAsync();

    Task FuturePromiseAsync();

    Task ReadWriteLockAsync();

    Task ProducerConsumerAsync();

    Task ActiveObjectAsync();
}