using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(UnitCommandContainer), typeof(UnitCommandScheduler), typeof(UnitCommandInvoker))]
public class UnitEntity : MonoBehaviour, IUnitEntity
{
    public UnitCommandContainer Container { get; private set; }
    public UnitCommandScheduler Scheduler { get; private set; }
    public UnitCommandInvoker Invoker { get; private set; }
    public UnitCommandPathDrawer PathDrawer { get; private set; }

    protected virtual void Awake() {
        Container = GetComponent<UnitCommandContainer>();
        Scheduler = GetComponent<UnitCommandScheduler>();
        Invoker = GetComponent<UnitCommandInvoker>();
        PathDrawer = GetComponent<UnitCommandPathDrawer>();
    }
    
    public virtual Task Move(Vector3 direction) {
        Debug.Log($"{name}: Move!");
        return Task.CompletedTask;
    }

    public virtual Task GatherResource(float searchSize) {
        Debug.Log($"{name}: Gather Resource!");
        return Task.CompletedTask;
    }

    public virtual Task Wait(float duration) {
        Debug.Log($"{name}: Wait!");
        return Task.CompletedTask;
    }

    public virtual Task DeliverResource(float searchSize) {
        Debug.Log($"{name}: Deliver Resource!");
        return Task.CompletedTask;
    }
}