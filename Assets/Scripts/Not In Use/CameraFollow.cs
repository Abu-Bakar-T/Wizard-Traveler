using UnityEngine;
// No Need
public class CameraFollow : MonoBehaviour
{
    public Transform target; // The player the camera will follow
    public Rigidbody targetRigidbody; // The player's Rigidbody for movement direction
    public float distance = 10.0f; // Default distance above the player
    public float smoothSpeed = 0.125f; // Speed of the camera adjustment
    public float collisionBuffer = 0.5f; // Buffer distance to avoid clipping

    private Vector3 smoothedPosition;
    private Quaternion initialRotation;

    void Start()
    {
        // Store the initial rotation of the camera
        initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (target == null || targetRigidbody == null)
            return;

        // Calculate the desired position directly above the player
        Vector3 desiredPosition = target.position + Vector3.up * distance;

        // Check for collisions
        RaycastHit hit;
        Vector3 direction = desiredPosition - target.position;
        if (Physics.Raycast(target.position, direction, out hit, distance))
        {
            if (hit.collider.gameObject != target.gameObject)
            {
                float hitDistance = Vector3.Distance(target.position, hit.point) - collisionBuffer;
                desiredPosition = target.position + direction.normalized * hitDistance;
            }
        }

        // Smoothly interpolate to the desired position
        smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Maintain the initial rotation
        transform.rotation = initialRotation;
    }
}
