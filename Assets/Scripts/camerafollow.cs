using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float distance = 15f;  // changed from 6f to start zoomed out
    public float height = 3f;
    public float smoothSpeed = 10f;

    [Header("Mouse Rotation")]
    public float mouseSensitivity = 3f;
    public float minVerticalAngle = -20f;
    public float maxVerticalAngle = 60f;

    [Header("Zoom")]
    public float minDistance = 2f;
    public float maxDistance = 80f;
    public float zoomSpeed = 2f;

    private float yaw = 0f;
    private float pitch = 20f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // Zoom
        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * distance;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // Your original code untouched
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, 0f, -distance);
        Vector3 targetPos = player.position + Vector3.up * height + offset;

        transform.position = Vector3.Lerp(
            transform.position, targetPos, Time.deltaTime * smoothSpeed);

        transform.LookAt(player.position + Vector3.up * 1f);
    }
}