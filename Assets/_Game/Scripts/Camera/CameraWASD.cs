using UnityEngine;

public class CameraWASD : CameraMovement
{
    public CameraWASD(Transform transform, float speed) : base(transform, speed) { }
    
    public override void GetInputs() {
        playerInput = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) playerInput.z = 1f;
        else if (Input.GetKey(KeyCode.S)) playerInput.z = -1f;
        
        if (Input.GetKey(KeyCode.A)) playerInput.x = -1f;
        else if (Input.GetKey(KeyCode.D)) playerInput.x = 1f;
    }

    public override void MoveCamera() {
        ApplyMovement(playerInput);
    }
}