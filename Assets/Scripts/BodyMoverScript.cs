using UnityEngine;

public class BodyMoverScript : MonoBehaviour
{
    public Transform cameraTransform;
    private float heightOffset = -1.7f;
    private float forwardOffset = -0.2f;
    public float rotationSmoothSpeed = 10f;

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Get the camera's forward direction projected on the ground
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        // Calculate desired position
        Vector3 targetPos = cameraTransform.position 
                            + camForward * forwardOffset
                            + Vector3.up * heightOffset;
        transform.position = targetPos;

        // Rotate body to face same direction
        if (camForward.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(camForward, Vector3.up),
                Time.deltaTime * 5f
            );
    }
}
