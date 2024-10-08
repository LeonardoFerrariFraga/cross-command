using System.Threading.Tasks;
using UnityEngine;

public interface ICommand
{
    public bool IsExecuting { get; }
    Task Execute();
    Task Undo();
}