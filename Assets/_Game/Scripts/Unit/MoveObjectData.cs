using UnityEngine;

[CreateAssetMenu(fileName = "MoveObjectData", menuName = "Commands/Data/Move Object")]
public class MoveObjectData : ScriptableObject
{
    [field: SerializeField] public float Duration { get; private set; } = 0.25f;
    [field: SerializeField] public float JumpHeight { get; private set; }
    [field: SerializeField] public float SquashEffector { get; private set; }
        
    [field: SerializeField] public AnimationCurve WalkCurve { get; private set; } = new(new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f));
    [field: SerializeField] public AnimationCurve RotationCurve { get; private set; } = new(new Keyframe(0f, 0f, 1f, 1f), new Keyframe(1f, 1f, 1f, 1f));

    [field: SerializeField] public AnimationCurve JumpCurve { get; private set; } = new(
        new Keyframe(0f, 0f, 0f, 5f),
        new Keyframe(0.5f, 1f, 0f, 0f),
        new Keyframe(1f, 0f, -5f, 1f));
        
    [field: SerializeField] public AnimationCurve SquashCurve { get; private set; } = new (
        new Keyframe(0f, 0f, 0f, 7f), 
        new Keyframe(0.4f, 1f, 0f, 0f), 
        new Keyframe(0.7f, 0f, -4f, -4f), 
        new Keyframe(0.8f, -0.3f, 0f, 0f), 
        new Keyframe(0.95f, 0.15f, 0f, 0f), 
        new Keyframe(1f, 0f, -5f, 1f));
}