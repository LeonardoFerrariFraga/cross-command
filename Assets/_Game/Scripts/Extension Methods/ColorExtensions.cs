using UnityEngine;

public static class ColorExtensions
{
    public static Color Add(this ref Color color, float value) => color = new Color(color.r + value, color.g + value, color.b + value);
}