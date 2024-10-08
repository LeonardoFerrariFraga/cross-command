using System.Threading.Tasks;
using UnityEngine;

public class UnitMoveAndJumpCommand : UnitCommand
{
    readonly Vector3 _direction;

    public UnitMoveAndJumpCommand(IUnitEntity entity, Vector3 direction) : base(entity) => _direction = direction; 
    public override async Task Execute() {
        IsExecuting = true;
        await entity.MoveAndJump(_direction);
        IsExecuting = false;
    }
    
    public override async Task Undo() {
        IsExecuting = true;
        await entity.MoveAndJump(-_direction);
        IsExecuting = false;
    }
}