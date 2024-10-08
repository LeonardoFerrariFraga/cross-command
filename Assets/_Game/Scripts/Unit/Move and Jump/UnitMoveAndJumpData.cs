using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitMoveAndJumpData", menuName = "Behaviour/Data/Unit Move and Jump")]
public class UnitMoveAndJumpData : ScriptableObject
{
    [field: SerializeField] public UnitMoveBehaviour moveBehaviour { get; private set; }
    [field: SerializeField, InlineEditor] public UnitMoveData moveData { get; private set; }
    
    [field: SerializeField, Space(15)] public UnitJumpBehaviour jumpBehaviour { get; private set; }
    [field: SerializeField, InlineEditor] public UnitJumpData jumpData { get; private set; }
}