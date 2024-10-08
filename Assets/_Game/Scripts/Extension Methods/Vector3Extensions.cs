using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 Add(this ref Vector3 vector, float value) => vector = new Vector3(vector.x + value, vector.y + value, vector.z + value);
    public static Vector3 Add(this ref Vector3 vector, float? x = null, float? y = null, float? z = null) {
        vector = new Vector3(vector.x + x ?? 0, vector.y + y ?? 0, vector.z + z ?? 0);
        return vector;
    }
}