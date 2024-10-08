using UnityEngine;

public static class TransformExtensions
{
    public static void AddPosition(this Transform transform, float? x = null, float? y = null, float? z = null) {
        transform.position += new Vector3(x ?? 0, y ?? 0, z ?? 0);
    }
    
    public static void SetPosition(this Transform transform, float? x = null, float? y = null, float? z = null) {
        Vector3 position = transform.position;
        transform.position = new Vector3(x ?? position.x, y ?? position.y, z ?? position.z);
    }
}