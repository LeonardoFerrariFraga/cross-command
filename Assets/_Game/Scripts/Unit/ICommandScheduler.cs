public interface ICommandScheduler
{
    int Count { get; }
    void Schedule(ICommand command);
    ICommand GetNextCommand();
    void CancelLastSchedule();
}