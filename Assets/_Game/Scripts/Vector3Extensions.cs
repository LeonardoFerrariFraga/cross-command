using UnityEngine;

public static class Vector3Extensions
{
    public static void AddVariation(this Vector3 vector, MinMaxFloat minMax) => vector.Add(minMax.RandomInBetween());
    public static Vector3 Add(this Vector3 vector, float value) => new Vector3(vector.x + value, vector.y + value, vector.z + value);
}

public static class ColorExtensions
{
    public static void AddVariation(this ref Color color, MinMaxFloat minMax) => color.Add(minMax.RandomInBetween());
    public static void Add(this ref Color color, float value) => color = new Color(color.r + value, color.g + value, color.b + value);
}