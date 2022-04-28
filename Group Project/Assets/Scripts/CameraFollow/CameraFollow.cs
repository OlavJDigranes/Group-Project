using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    // Didn't know this existed, LateUpdate runs AFTER any Update method.
    private void FixedUpdate()
    {
        // Get the target's position (add any offsets if needed) and save it to a vector3 var
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly follow the desiredPosition according to the smoothSpeed
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // APply the smoothedPosition to the Camera
        transform.position = smoothedPosition;
    }
}
