using System.Threading.Tasks;
using UnityEngine;

public class GlobalCommandInvoker : MonoBehaviour
{
    static GlobalCommandInvoker Instance;
    
    UnitEntity[] _unitEntities;
    bool _isExecuting;

    void Awake() {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        
        _unitEntities = FindObjectsByType<UnitEntity>(FindObjectsSortMode.None);
    }

    void Update() {
        if (_isExecuting) return;
        
        if (Input.GetKeyDown(KeyCode.Space)) {
            ExecuteAll();
        }
    }
    
    async void ExecuteAll() {
        if (_unitEntities is not { Length: > 0 }) return;
        
        _isExecuting = true;
        Task[] commandTasks = new Task[_unitEntities.Length];
        for (int i = 0; i < commandTasks.Length; i++) {
            commandTasks[i] = _unitEntities[i].Invoker.ExecuteAllAsync(_unitEntities[i].Scheduler);
        }

        await Task.WhenAll(commandTasks);
        _isExecuting = false;
    }
}