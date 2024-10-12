using UnityEngine;

public class CameraPanScrolling : CameraMovement
{
    // The pan speed is much greater than the other camera movement
    // This is an arbitrary number to slower the speed effect on this script to match the others
    const float SPEED_MULTIPLIER = 0.04f;
    
    public CameraPanScrolling(Transform transform, float speed) : base(transform, speed) { }
    
    public override void GetInputs() {
        if (Input.GetMouseButtonDown(2)) {
            dragging = true;
        }    
        else if (Input.GetMouseButtonUp(2)) {
            dragging = false;
        }

        if (dragging) {
            playerInput = Input.mousePositionDelta;
        }
    }

    public override void MoveCamera() {
        if (!dragging) return;
        
        playerInput = -playerInput;
        (playerInput.z, playerInput.y) = (playerInput.y, 0f);
     
        ApplyMovement(playerInput * SPEED_MULTIPLIER);
    }
}