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
        
        Vector3 movementInput = Vector3.zero;
        if (mousePos.x < EDGE_SIZE) movementInput.x = -1f;
        else if (mousePos.x > 1f - EDGE_SIZE) movementInput.x = 1f;
        
        if (mousePos.y < EDGE_SIZE) movementInput.z = -1f;
        else if (mousePos.y > 1f - EDGE_SIZE) movementInput.z = 1f;
            
        if (movementInput != Vector3.zero)
            ApplyMovement(movementInput);
    }
}