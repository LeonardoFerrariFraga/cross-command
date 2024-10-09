using UnityEngine;

[CreateAssetMenu(fileName = "UnitMoveData", menuName = "Behaviour/Data/Unit Move")]
public class UnitMoveData : ScriptableObject
{
    [field: SerializeField] public float duration {get; private set;}
    [field: SerializeField] public AnimationCurve walkCurve {get; private set;}
    [field: SerializeField] public AnimationCurve rotationCurve {get; private set;}
}