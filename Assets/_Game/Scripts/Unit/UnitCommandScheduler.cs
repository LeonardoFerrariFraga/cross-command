using System.Collections.Generic;
using UnityEngine;

public class UnitCommandScheduler : MonoBehaviour, ICommandScheduler
{
    Queue<ICommand> _commands = new();
    public int Count => _commands.Count;
    
    public void Schedule(ICommand command) => _commands.Enqueue(command);
    public ICommand GetNextCommand() => _commands is { Count: > 0 } ? _commands.Dequeue() : null;
    public void CancelLastSchedule() => _commands.Dequeue();
}