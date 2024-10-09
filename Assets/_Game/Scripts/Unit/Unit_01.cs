using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class Unit_01 : MonoBehaviour, IUnitEntity
{
    [SerializeField, InlineEditor] UnitAnimatedMoveBehaviour _moveBehaviour;
    
    public async Task Move(Vector3 direction) {
        CancellationToken cancellationToken = new();
        await _moveBehaviour.AnimatedMove(transform, direction, cancellationToken);
    }

    public Task GatherResource(float searchSize) {
        throw new System.NotImplementedException();
    }

    public Task Wait(float duration) {
        throw new System.NotImplementedException();
    }

    public Task DeliverResource(float searchSize) {
        throw new System.NotImplementedException();
    }
}