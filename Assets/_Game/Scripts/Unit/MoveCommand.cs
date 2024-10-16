using UnityEngine;

[CreateAssetMenu(fileName = "MoveCommand", menuName = "SO Commands/Move Command")]
public class MoveCommand : ScriptableObjectCommand
{
    [SerializeField] protected Vector3Int _direction;
    
#if UNITY_EDITOR
    void OnValidate() {
        if (_direction.x is < -1 or > 1)
            _direction.x = (int)Mathf.Clamp(_direction.x, -1f, 1f);

        if (_direction.y is < -1 or > 1)
            _direction.y = (int)Mathf.Clamp(_direction.y, -1f, 1f);
    }
#endif

    public override ICommand CreateCommand(IUnitEntity entity) => new UnitAnimatedMoveCommand(entity, _direction);
}