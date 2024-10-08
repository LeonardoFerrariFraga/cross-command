using System.Threading.Tasks;
using UnityEngine;

public interface IUnitEntity
{
    Task Move(Vector3 direction);
    Task Jump();
    Task MoveAndJump(Vector3 direction);
}