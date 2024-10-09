using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class UnitAnimatedMoveCommand : UnitCommand
{
    Vector3 _direction;

    public UnitAnimatedMoveCommand(IUnitEntity entity, Vector3 direction) : base(entity) => _direction = direction;
    
    public override async Task Execute() {
        IsExecuting = true;
        await entity.Move(_direction);
        IsExecuting = false;
    }
    
    public override async Task Undo() {
        IsExecuting = true;
        await entity.Move(-_direction);
        IsExecuting = false;
    }
}