using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public abstract class UnitCommand : ICommand
{
    protected IUnitEntity entity;
    
    public bool IsExecuting { get; protected set; }

    protected UnitCommand(IUnitEntity entity) => this.entity = entity;
    
    public abstract Task Execute();

    public virtual Task Undo() {
        Debug.Log($"{GetType().Name}: Undo() not implemented.");
        return Task.CompletedTask;
    }
}