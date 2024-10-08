using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitMoveBehaviour", menuName = "Behaviour/Unit Move")]
public class UnitMoveBehaviour : ScriptableObject
{
    public async Task Move(Transform transform, Vector3 direction, UnitMoveData data, CancellationToken cancellationToken) {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + direction;
        
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.LookRotation(endPos - startPos, Vector3.up);
        
        float elapsedTime = 0f;
        while (elapsedTime < data.duration) {
            float t = elapsedTime / data.duration;
            Vector3 position = Vector3.Lerp(startPos, endPos, data.walkCurve.Evaluate(t));
            position.y = transform.position.y;
            transform.position = position;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, data.rotationCurve.Evaluate(t));
            
            elapsedTime += Time.deltaTime;
            await Awaitable.NextFrameAsync(cancellationToken);
        }

        transform.position = endPos;
        transform.rotation = endRotation;
    }
}