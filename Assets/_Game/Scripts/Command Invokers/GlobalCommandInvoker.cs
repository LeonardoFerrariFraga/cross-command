using System.Threading.Tasks;
using UnityEngine;

public class GlobalCommandInvoker : MonoBehaviour
{
    static GlobalCommandInvoker Instance;
    
    CommandInvoker[] _invokers;
    bool _isExecuting;

    void Awake() {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        
        _invokers = FindObjectsByType<CommandInvoker>(FindObjectsSortMode.None);
    }

    void Update() {
        if (_isExecuting) return;
        
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            ExecuteAll();
        }
    }
    
    async void ExecuteAll() {
        if (_invokers is not { Length: > 0 }) return;
        
        _isExecuting = true;
        Task[] commandTasks = new Task[_invokers.Length];
        for (int i = 0; i < commandTasks.Length; i++) {
            commandTasks[i] = _invokers[i].ExecuteAll();
        }

        await Task.WhenAll(commandTasks);
        _isExecuting = false;
    }
}