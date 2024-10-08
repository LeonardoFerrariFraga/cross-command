using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class Unit_01 : MonoBehaviour, IUnitEntity
{
    [SerializeField] UnitMoveBehaviour _moveBehaviour;
    [SerializeField, InlineEditor] UnitMoveData _moveData;
    
    ICommand _commandForward;
    ICommand _commandBackward;

    Stack<ICommand> _undoComments = new();
    Stack<ICommand> _redoComments = new();
    
    void Start() {
        _commandForward = new MoveUnitCommand(this, Vector3.forward);
        _commandBackward = new MoveUnitCommand(this, -Vector3.forward);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.W) && !_commandForward.IsExecuting) {
            _commandForward.Execute();
            _undoComments.Push(_commandForward);
        }
        else if (Input.GetKeyDown(KeyCode.S) && !_commandBackward.IsExecuting) {
            _commandBackward.Execute();
            _undoComments.Push(_commandBackward);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && _undoComments is { Count: > 0 }) {
            ICommand command = _undoComments.Pop();
            command.Undo();
            _redoComments.Push(command);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && _redoComments is { Count: > 0 }) {
            ICommand command = _redoComments.Pop();
            command.Execute();
            _undoComments.Push(command);
        }
    }

    public async Task Move(Vector3 direction) {
        CancellationToken cancellationToken = new ();
        await _moveBehaviour.Move(transform, direction, _moveData, cancellationToken);
    }
}