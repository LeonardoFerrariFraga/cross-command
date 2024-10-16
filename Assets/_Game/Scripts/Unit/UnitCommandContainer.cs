using UnityEngine;

[RequireComponent(typeof(IUnitEntity))]
public class UnitCommandContainer : MonoBehaviour
{
    [SerializeField] ScriptableObjectCommand[] _commands;
    public ScriptableObjectCommand[] Commands => _commands;
}