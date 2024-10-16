using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UnitCommandInvoker : MonoBehaviour, ICommandInvoker
{
    public event Action OnStartExecutingAll;
    public event Action OnStartExecutingNext;
    public event Action OnEndExecutingAll;

    bool _isExecuting;
    public bool IsExecuting {
        get => _isExecuting;
        private set {
            _isExecuting = value;
            if (value)
                OnStartExecutingAll?.Invoke();
            else 
                OnEndExecutingAll?.Invoke();
        }
    }
    
    public async Task ExecuteAllAsync(ICommandScheduler scheduler) {
        IsExecuting = true;
        while (scheduler.Count > 0) {
            OnStartExecutingNext?.Invoke();
            await scheduler.GetNextCommand().Execute();
        }

        IsExecuting = false;
    }

    public Task[] GetAllExecutionTasks(ICommandScheduler scheduler) {
        IsExecuting = true;
        
        List<Task> tasks = new List<Task>(scheduler.Count);
        while (scheduler.Count > 0) {
            OnStartExecutingNext?.Invoke();
            tasks.Add(scheduler.GetNextCommand().Execute());
        }

        ControlIsExecutingFlag(tasks.ToArray());
        return tasks.ToArray();
    }

    async void ControlIsExecutingFlag(Task[] tasks) {
        await Task.WhenAll(tasks);
        IsExecuting = false;
    }
}