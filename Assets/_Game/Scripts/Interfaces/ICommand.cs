using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public interface ICommand
{
    public bool IsExecuting { get; }
    Task Execute();

    Task Undo() {
        Debug.Log("Default Undo");
        return Task.CompletedTask;
    }

    public static ICommand Empty() => new EmptyCommand();
}

public class EmptyCommand : ICommand
{
    public bool IsExecuting { get; }
    public Task Execute() {
        Debug.Log($"Empty command...");
        return Task.CompletedTask;
    }
}