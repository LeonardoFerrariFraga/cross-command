using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class Unit_01 : MonoBehaviour, IUnitEntity
{
    [SerializeField] UnitMoveBehaviour _moveBehaviour;
    [SerializeField, InlineEditor] UnitMoveData _moveData;
    [Space(15)]
    [SerializeField] UnitJumpBehaviour _jumpBehaviour;
    [SerializeField, InlineEditor] UnitJumpData _jumpData;
    [Space(15)]
    [SerializeField] UnitMoveAndJumpBehaviour _moveJumpBehaviour;
    [SerializeField, InlineEditor] UnitMoveAndJumpData _moveAndJumpData;
    
    ICommand _commandForward;
    ICommand _commandBackward;
    ICommand _commandJump;
    ICommand _commandMoveAndJumpForward;
    ICommand _commandMoveAndJumpBackward;

    readonly Stack<ICommand> _undoComments = new();
    readonly Stack<ICommand> _redoComments = new();

    void Start() {
        _commandForward = new UnitMoveCommand(this, Vector3.forward);
        _commandBackward = new UnitMoveCommand(this, -Vector3.forward);
        _commandJump = new UnitJumpCommand(this);
        _commandMoveAndJumpForward = new UnitMoveAndJumpCommand(this, Vector3.forward);
        _commandMoveAndJumpBackward = new UnitMoveAndJumpCommand(this, -Vector3.forward);
    }

    void Update() {
        if (_commandForward.IsExecuting || _commandBackward.IsExecuting || _commandJump.IsExecuting)
            return;
        
        if (Input.GetKeyDown(KeyCode.W)) {

            _commandMoveAndJumpForward.Execute();
            _undoComments.Push(_commandMoveAndJumpForward);
        }
        else if (Input.GetKeyDown(KeyCode.S)) {
            _commandMoveAndJumpBackward.Execute();
            _undoComments.Push(_commandMoveAndJumpBackward);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && _undoComments is { Count: > 0 }) {
            ICommand command = _undoComments.Pop();
            command.Undo();
            _commandJump.Execute();
            _redoComments.Push(command);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && _redoComments is { Count: > 0 }) {
            ICommand command = _redoComments.Pop();
            command.Execute();
            _commandJump.Execute();
            _undoComments.Push(command);
        }
    }

    public async Task Move(Vector3 direction) {
        CancellationToken cancellationToken = new ();
        await _moveBehaviour.Move(transform, direction, _moveData, cancellationToken);
    }

    public async Task Jump() {
        CancellationToken cancellationToken = new();
        await _jumpBehaviour.Jump(transform, _jumpData, cancellationToken);
    }

    public async Task MoveAndJump(Vector3 direction) {
        CancellationToken cancellationToken = new();
        await _moveJumpBehaviour.MoveAndJump(transform, direction, _moveAndJumpData, cancellationToken);
    }
}