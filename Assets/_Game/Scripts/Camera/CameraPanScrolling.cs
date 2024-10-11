using UnityEngine;

public class CameraPanScrolling : CameraMovement
{
    bool _dragging;
    Vector2 _lastMousePosition;

    // The pan speed is much greater than the other camera movement
    // This is an arbitrary number to slower the speed effect on this script to match the others
    const float SPEED_MULTIPLIER = 0.04f;
    
    public CameraPanScrolling(Transform transform, float speed) : base(transform, speed) { }
    
    public override void GetInputs() {
        if (Input.GetMouseButtonDown(2) && !_dragging) {
            _dragging = true;
            _lastMousePosition = Input.mousePosition;
        }    
        else if (Input.GetMouseButtonUp(2) && _dragging) {
            _dragging = false;
        }

        playerInput = Input.mousePosition;
    }

    public override void MoveCamera() {
        if (!_dragging) return;
        
        Vector3 mouseMoveDelta = (Vector2)playerInput - _lastMousePosition;
        mouseMoveDelta.z = mouseMoveDelta.y;
        
        _lastMousePosition = Input.mousePosition;
        
        ApplyMovement(-mouseMoveDelta * SPEED_MULTIPLIER);
    }
}