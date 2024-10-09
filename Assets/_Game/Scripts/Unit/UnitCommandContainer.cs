using System;
using UnityEngine;

[RequireComponent(typeof(IUnitEntity))]
public class UnitCommandContainer : MonoBehaviour
{
    IUnitEntity _entity;
    (Func<ICommand> command, string name)[] _commands;
    public (Func<ICommand> command, string name)[] Commands => _commands;
    
    void Awake() {
        _entity = GetComponent<IUnitEntity>();
        
        _commands = new (Func<ICommand>, string)[4];
        _commands[0] = (CreateForwardCommand, "Forward");
        _commands[1] = (CreateBackwardCommand, "Backward");
        _commands[2] = (CreateRightCommand, "Right");
        _commands[3] = (CreateLeftCommand, "Left");
    }

    ICommand CreateForwardCommand() => Create(_entity, Vector3.forward);
    ICommand CreateBackwardCommand() => Create(_entity, -Vector3.forward);
    ICommand CreateRightCommand() => Create(_entity, Vector3.right);
    ICommand CreateLeftCommand() => Create(_entity, -Vector3.right);
    ICommand Create(IUnitEntity entity, Vector3 direction) => new UnitAnimatedMoveCommand(entity, direction);
}