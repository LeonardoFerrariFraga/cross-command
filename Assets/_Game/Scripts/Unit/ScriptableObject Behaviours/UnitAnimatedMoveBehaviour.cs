using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitAnimatedMoveBehaviour", menuName = "Behaviour/Unit/Animated Move")]
public class UnitAnimatedMoveBehaviour : ScriptableObject
{
    [SerializeField] UnitAnimatedMoveData _data;
    
    public async Task AnimatedMove(Transform transform, Vector3 direction, CancellationToken cancellationToken) {
        Task move = _data.moveBehaviour.Move(transform, direction, _data.moveData, cancellationToken);
        Task jump = _data.jumpBehaviour.Jump(transform, _data.jumpData, cancellationToken);
        await Task.WhenAll(move, jump);
    }
}