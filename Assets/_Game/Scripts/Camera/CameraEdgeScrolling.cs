using UnityEngine;

public class CameraEdgeScrolling : CameraMovement
{
    const float EDGE_SIZE = 0.05f;
    readonly Camera _camera;
    
    public CameraEdgeScrolling(Transform transform, float speed) : base(transform, speed) {
        _camera = Object.FindAnyObjectByType<Camera>();
    }

    public override void GetInputs() => playerInput = Input.mousePosition;
    
    public override void MoveCamera() {
        Vector3 mousePos = _camera.ScreenToViewportPoint(playerInput);

        if (!AtEdge(mousePos.x) && !AtEdge(mousePos.y))
            return;

        mousePos.x = Mathf.Lerp(-1f, 1f, mousePos.x);
        mousePos.z = Mathf.Lerp(-1f, 1f, mousePos.y);
        
        ApplyMovement(mousePos);
    }

    bool AtEdge(float axisValue) {
        return axisValue is <= EDGE_SIZE or >= 1f - EDGE_SIZE;
    }
}