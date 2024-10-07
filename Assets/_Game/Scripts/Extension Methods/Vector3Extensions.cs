using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 Add(this ref Vector3 vector, float value) => vector = new Vector3(vector.x + value, vector.y + value, vector.z + value);
}