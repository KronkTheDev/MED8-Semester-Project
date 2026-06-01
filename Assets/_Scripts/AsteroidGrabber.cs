using UnityEngine;
using UnityEngine.InputSystem;

public class AsteroidGrabber : MonoBehaviour {
    [Header("Settings")]
    public float throwForce = 15f;    
    
    private GameObject grabbedObject;
    private Rigidbody grabbedRb;
    private Camera cam;
    private float currentGrabDistance; // New variable to store the unique distance of the hit object

    void Start() {
        cam = Camera.main;
    }

    void Update() {
        bool mousePressed = Mouse.current.leftButton.wasPressedThisFrame;
        bool mouseHeld = Mouse.current.leftButton.isPressed;
        bool mouseReleased = Mouse.current.leftButton.wasReleasedThisFrame;

        // 1. Try to grab
        if (mousePressed) {
            TryGrabObject();
        }

        // 2. While Holding
        if (grabbedObject != null && mouseHeld) {
            HoldObject();

            // NEW: Rotate object while holding using Scroll Wheel
            float rotateInput = Mouse.current.scroll.ReadValue().y;
            if (rotateInput != 0) {
                // Adjust the 0.1f to make rotation faster or slower
                grabbedObject.transform.Rotate(Vector3.up, rotateInput * 0.1f);
            }
        }

        // 3. Release/Throw
        if (mouseReleased && grabbedObject != null) {
            ReleaseObject();
        }
    }

    void TryGrabObject() {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f)) {
            // Check for BOTH Asteroids and Life
            if (hit.collider.CompareTag("Asteroid") || hit.collider.CompareTag("Life")) {
                grabbedObject = hit.collider.gameObject;
                grabbedRb = grabbedObject.GetComponent<Rigidbody>();

                // FIX: Calculate the exact distance from the camera to the object at the moment of impact
                currentGrabDistance = Vector3.Distance(cam.transform.position, hit.point);

                // Detach from planet if it was already placed
                grabbedObject.transform.SetParent(null); 
                
                if (grabbedRb != null) {
                    grabbedRb.isKinematic = true; 
                    grabbedRb.linearVelocity = Vector3.zero;
                }

                // Tell the stick script we are moving it
                if (grabbedObject.TryGetComponent(out MagneticStick stick)) {
                    stick.isGrabbed = true;
                }
            }
        }
    }

    void HoldObject() {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        
        // Use currentGrabDistance instead of a fixed value so the object stays where you grabbed it
        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, currentGrabDistance));

        grabbedObject.transform.position = worldPos;
    }

    void ReleaseObject() {
        // Tell the stick script we let go so it can snap to the surface
        if (grabbedObject.TryGetComponent(out MagneticStick stick)) {
            stick.isGrabbed = false;
        }

        if (grabbedRb != null) {
            grabbedRb.isKinematic = false; 
            grabbedRb.AddForce(cam.transform.forward * throwForce, ForceMode.Impulse);
        }

        grabbedObject = null;
        grabbedRb = null;
    }
}