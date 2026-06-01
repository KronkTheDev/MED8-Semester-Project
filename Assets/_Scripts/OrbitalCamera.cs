using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbit : MonoBehaviour {
    [Header("Target Settings")]
    public Transform target; // Drag your Planet/Sun here
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 100f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 0.2f;

    private float currentZoom = 20f;
    private Vector2 rotationValues;

    void Start() {
        if (target == null) {
            Debug.LogWarning("CameraOrbit: No target assigned! Please drag the Planet into the Target slot.");
            return;
        }
        
        // Initialize rotation based on current camera position
        Vector3 angles = transform.eulerAngles;
        rotationValues.x = angles.y;
        rotationValues.y = angles.x;
    }

    void LateUpdate() {
        if (!target) return;

        // 1. Handling Zoom (Mouse Scroll)
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0) {
            // New Input System scroll values are often large (e.g. 120), so we multiply by a small factor
            currentZoom -= scroll * 0.01f * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        }

        // 2. Handling Rotation (Right Click + Drag)
        if (Mouse.current.rightButton.isPressed) {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            rotationValues.x += mouseDelta.x * rotationSpeed;
            rotationValues.y -= mouseDelta.y * rotationSpeed;

            // Optional: Limit vertical rotation so you don't flip upside down
            rotationValues.y = Mathf.Clamp(rotationValues.y, -89f, 89f);
        }

        // 3. Apply the Position and Rotation
        Quaternion rotation = Quaternion.Euler(rotationValues.y, rotationValues.x, 0);
        Vector3 position = target.position - (rotation * Vector3.forward * currentZoom);

        transform.rotation = rotation;
        transform.position = position;
    }
}