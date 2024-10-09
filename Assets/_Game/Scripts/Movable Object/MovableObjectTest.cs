using Sirenix.OdinInspector;
using UnityEngine;

public class MovableObjectTest : MonoBehaviour
{
    [SerializeField] bool _useUnitData;
    
    [SerializeField, HideIf("_useUnitData"), BoxGroup("Fields")] float _duration;
    [SerializeField, HideIf("_useUnitData"), BoxGroup("Fields")] float _jumpHeight;
    [SerializeField, HideIf("_useUnitData"), BoxGroup("Fields")] float _squashEffector;
    [SerializeField, HideIf("_useUnitData"), BoxGroup("Fields")] AnimationCurve _walkCurve;
    [SerializeField, HideIf("_useUnitData"), BoxGroup("Fields")] AnimationCurve _rotationCurve;
    [SerializeField, HideIf("_useUnitData"), BoxGroup("Fields")] AnimationCurve _jumpCurve;
    [SerializeField, HideIf("_useUnitData"), BoxGroup("Fields")] AnimationCurve _squashCurve;
    
    [SerializeField, ShowIf("_useUnitData"), InlineEditor] MoveObjectData _data;
    
    ICommand currentCommand;

    void Start() => AlignWithGround();
    void AlignWithGround() {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, 1000f, LayerMask.GetMask($"Ground"));
        Vector3 pos = transform.position;
        transform.position = new Vector3((int)pos.x, hitInfo.point.y + transform.localScale.y / 2f, (int)pos.z);
    }
    
    void Update() {
        if (currentCommand is { IsExecuting: true })
            return;
        
        Vector3 direction = Vector3.zero;
        
        if (Input.GetKeyDown(KeyCode.W)) {
            direction = Vector3.forward;    
        }
        else if (Input.GetKeyDown(KeyCode.S)){
            direction = -Vector3.forward;
        }
        else if (Input.GetKeyDown(KeyCode.D)){
            direction = Vector3.right;
        }
        else if (Input.GetKeyDown(KeyCode.A)){
            direction = -Vector3.right;
        }

        if (direction == Vector3.zero)
            return;
        
        if (_useUnitData)
            currentCommand = new MoveObjectCommand.Builder().Build(transform, direction, _data);
        else {
            currentCommand = new MoveObjectCommand.Builder()
                .WithDuration(_duration)
                .WithJumpHeight(_jumpHeight)
                .WithSquashEffector(_squashEffector)
                .WithWalkCurve(_walkCurve)
                .WithRotationCurve(_rotationCurve)
                .WithJumpCurve(_jumpCurve)
                .WithSquashCurve(_squashCurve)
                .Build(transform, direction);    
        }
        
        currentCommand.Execute();
    }
}