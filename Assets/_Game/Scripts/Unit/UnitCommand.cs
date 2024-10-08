using System.Threading.Tasks;

public abstract class UnitCommand : ICommand
{
    protected IUnitEntity entity;
    public bool IsExecuting { get; protected set; }

    protected UnitCommand(IUnitEntity entity) => this.entity = entity;
    
    public abstract Task Execute();
    public abstract Task Undo();
}