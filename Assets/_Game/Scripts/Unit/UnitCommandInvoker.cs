using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UnitCommandInvoker : CommandInvoker
{
    [SerializeField] int _undoSize = 5;
    
    Stack<ICommand> _commands = new();
    LimitedStack<ICommand> _undoCommand;
    List<ICommand> _redoCommand = new();
    bool _isExecuting;

    #if UNITY_EDITOR
    void OnValidate() => _undoSize = Mathf.Clamp(_undoSize, 1, int.MaxValue);
    #endif
    
    void Awake() {
        _undoCommand = new LimitedStack<ICommand>(_undoSize);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && !_isExecuting) {
            _ = ExecuteAll();
        }
    }
    
    public override void Schedule(ICommand command) => _commands.Push(command);
    public override void CancelLastSchedule() => _commands.Pop();
    public override async Task ExecuteAll() {
        if (_commands is not { Count: > 0 }) return;
        
        _isExecuting = true;
        while (_commands is { Count: > 0 })
            await _commands.Pop().Execute();
        
        _isExecuting = false;
    }
}