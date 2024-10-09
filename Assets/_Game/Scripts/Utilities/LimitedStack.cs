using System;
using System.Collections.Generic;

public class LimitedStack<T>
{
    LinkedList<T> _commands;
    int _size;

    public int Count => _commands.Count;
    public bool IsEmpty => _commands.Count == 0;
    
    public LimitedStack(int size) {
        if (size <= 0)
            throw new InvalidOperationException("CommandStack<T> size must be greater than 0.");
        
        _size = size;
        _commands = new LinkedList<T>();
    }
    
    public T Pop() {
        T command = _commands.Last.Value;
        _commands.RemoveLast();
        return command;
    }

    public void Push(T command) {
        if (_commands.Count == _size)
            _commands.RemoveFirst();
        
        _commands.AddLast(command);
    }

    public T Peek() {
        if (_commands is { Count: > 0 })
            return _commands.Last.Value;

        return default;
    }
}