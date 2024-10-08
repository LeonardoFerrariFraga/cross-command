using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitJumpData", menuName = "Behaviour/Data/Unit Jump")]
public class UnitJumpData : ScriptableObject
{
    [field: SerializeField, SuffixLabel("Usually bigger than the Jump Duration...", overlay: true)] 
    public float squashDuration {get; private set;}
    
    [field: SerializeField, SuffixLabel("Usually same as Move Duration (if has it)...", overlay: true)] 
    public float jumpDuration {get; private set;}
    
    [field: SerializeField] public float jumpHeight {get; private set;}
    [field: SerializeField] public float squashEffector {get; private set;}
    [field: SerializeField] public AnimationCurve jumpCurve {get; private set;}
    [field: SerializeField] public AnimationCurve squashCurve {get; private set;}
}