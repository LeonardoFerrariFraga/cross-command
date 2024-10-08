using System.Threading.Tasks;
using UnityEngine;

public interface IUnitEntity
{
    Task Move(Vector3 direction);
}