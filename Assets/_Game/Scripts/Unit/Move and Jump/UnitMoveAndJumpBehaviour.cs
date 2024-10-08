using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitMoveAndJumpBehaviour", menuName = "Behaviour/Unit/Move and Jump")]
public class UnitMoveAndJumpBehaviour : ScriptableObject
{
    public async Task MoveAndJump(Transform transform, Vector3 direction, UnitMoveAndJumpData data, CancellationToken cancellationToken) {
        Task move = data.moveBehaviour.Move(transform, direction, data.moveData, cancellationToken);
        Task jump = data.jumpBehaviour.Jump(transform, data.jumpData, cancellationToken);
        await Task.WhenAll(move, jump);
    }
}