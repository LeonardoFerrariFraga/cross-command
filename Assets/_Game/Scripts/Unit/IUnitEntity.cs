using UnityEngine;
using System.Threading.Tasks;

public interface IUnitEntity
{
    Task Move(Vector3 direction);
    Task GatherResource(float searchSize);
    Task Wait(float duration);
    Task DeliverResource(float searchSize);
}