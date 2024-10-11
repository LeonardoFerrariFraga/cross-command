using UnityEngine;

public abstract class CameraMovement
{
    Transform transform;
    float speed;

    protected Vector3 playerInput;
    
    protected CameraMovement(Transform transform, float speed) {
        this.transform = transform;
        this.speed = speed;
    }

    public abstract void GetInputs();
    public abstract void MoveCamera();
    protected void ApplyMovement(Vector3 movementInput) {
        Vector3 moveDir = movementInput.z * transform.forward + movementInput.x * transform.right;
        moveDir.y = 0;
        
        Vector3 position = transform.position;
        transform.position = Vector3.MoveTowards(position, position + moveDir, speed * Time.deltaTime);
    }
}