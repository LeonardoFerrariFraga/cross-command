using UnityEngine;

public abstract class ScriptableObjectCommand : ScriptableObject
{
    [field: SerializeField] public string Label { get; protected set;}
    public abstract ICommand CreateCommand(IUnitEntity entity);        
}