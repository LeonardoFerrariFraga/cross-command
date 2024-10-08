using System.Threading.Tasks;

public class UnitJumpCommand : UnitCommand
{
    public UnitJumpCommand(IUnitEntity entity) : base(entity) { }
    public override async Task Execute() {
        IsExecuting = true;
        await entity.Jump();
        IsExecuting = false;
    }
}