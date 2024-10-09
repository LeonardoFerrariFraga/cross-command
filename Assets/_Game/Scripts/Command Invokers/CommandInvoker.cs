using System.Threading.Tasks;
using UnityEngine;

public abstract class CommandInvoker : MonoBehaviour
{
    public abstract void Schedule(ICommand command);
    public abstract void CancelLastSchedule();
    public abstract Task ExecuteAll();
}