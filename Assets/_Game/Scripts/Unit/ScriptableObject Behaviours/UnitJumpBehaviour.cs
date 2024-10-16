using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitJumpBehaviour", menuName = "Behaviour/Unit/Jump")]
public class UnitJumpBehaviour : ScriptableObject
{
    public async Task Jump(Transform transform, UnitJumpData data, CancellationToken cancellationToken) {
        Task jump = Jump(transform, data.jumpDuration, data.jumpHeight, data.jumpCurve, cancellationToken);
        Task squash = Squash(transform, data.squashDuration, data.squashEffector, data.squashCurve, cancellationToken);
        await Task.WhenAll(jump, squash);
    }

    async Task Jump(Transform transform, float duration, float jumpHeight, AnimationCurve jumpCurve, CancellationToken cancellationToken) {
        Vector3 startScale = transform.localScale;
        float startY = transform.position.y;
        
        float elapsedTime = 0f;
        while (elapsedTime < duration) {

            float t = elapsedTime / duration;
            float posY = Mathf.Lerp(startY, startY + jumpHeight, jumpCurve.Evaluate(t));
            transform.SetPosition(y: posY);
            
            elapsedTime += Time.deltaTime;
            await Awaitable.NextFrameAsync(cancellationToken);
        }

        Vector3 pos = transform.position;
        pos.y = startY;
        transform.position = pos;
        
        transform.localScale = startScale;
    }

    async Task Squash(Transform transform, float duration, float squashEffector, AnimationCurve squashCurve, CancellationToken cancellationToken) {
        Vector3 startScale = transform.localScale;
        
        float elapsedTime = 0f;
        while (elapsedTime < duration) {

            float t = elapsedTime / duration;
            
            float multiplier = 1f + squashEffector * squashCurve.Evaluate(t);
            transform.localScale = new Vector3(startScale.x / multiplier, startScale.y * multiplier, startScale.z / multiplier);
            
            elapsedTime += Time.deltaTime;
            await Awaitable.NextFrameAsync(cancellationToken);
        }

        transform.localScale = startScale;
    }
}