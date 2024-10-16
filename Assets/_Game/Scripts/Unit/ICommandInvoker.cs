using System;
using System.Threading.Tasks;

public interface ICommandInvoker
{
    event Action OnStartExecutingAll;
    event Action OnStartExecutingNext;
    event Action OnEndExecutingAll;
    bool IsExecuting { get; }
    Task ExecuteAllAsync(ICommandScheduler scheduler);
    Task[] GetAllExecutionTasks(ICommandScheduler scheduler);
}