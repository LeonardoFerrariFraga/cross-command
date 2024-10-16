using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PathDrawer : MonoBehaviour, IUnitEntity
{
    [SerializeField] UnitMoveBehaviour _moveBehaviour;
    [SerializeField] UnitMoveData _moveData;
    
    public async Task Move(Vector3 direction) {
        CancellationToken cancellationToken = new();
        await _moveBehaviour.Move(transform, direction, _moveData, cancellationToken);
    }

    public Task GatherResource(float searchSize) {
        return Task.CompletedTask;
    }

    public Task Wait(float duration) {
        return Task.CompletedTask;
    }

    public Task DeliverResource(float searchSize) {
        return Task.CompletedTask;
    }
}