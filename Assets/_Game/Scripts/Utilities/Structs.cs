using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct MinMaxFloat
{
    [field: SerializeField] public float min { get; private set; }
    [field: SerializeField] public float max { get; private set; }

    // if Abs(A) - Abs(B) < COMPARISON_TOLERANCE, A and B are consider equals.
    const float COMPARISON_TOLERANCE = 0.0001f;
    
    public MinMaxFloat(float min, float max) => (this.min, this.max) = (min, max);
    
    public float RandomInBetween() => Random.Range(min, max);
    public bool IsMinOrMax(float value) => Math.Abs(value - min) < COMPARISON_TOLERANCE || Math.Abs(value - max) < COMPARISON_TOLERANCE;
}
