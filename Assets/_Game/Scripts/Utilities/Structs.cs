using UnityEngine;

[System.Serializable]
public struct MinMaxFloat
{
    [field: SerializeField] public float min { get; private set; }
    [field: SerializeField] public float max { get; private set; }

    public MinMaxFloat(float min, float max) => (this.min, this.max) = (min, max);
    
    public float RandomInBetween() => Random.Range(min, max);
}
