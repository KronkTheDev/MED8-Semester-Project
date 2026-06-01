using UnityEngine;

public class GravitySource : MonoBehaviour {
    [Header("Gravity Settings")]
    public float gravityIntensity = 50f; // Strength of the pull
    public float pullRadius = 20f;       // How far away it can "sense" asteroids

    void FixedUpdate() {
        // Calculate a dynamic radius based on the planet's scale
        // This ensures the gravity field grows as the planet gets bigger
        float dynamicRadius = pullRadius * transform.localScale.x;

        // 1. Find all colliders within the pull radius
        // Updated to use the dynamicRadius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, dynamicRadius);

        foreach (var hitCollider in hitColliders) {
            // 2. Check if the object has a Rigidbody and isn't itself
            Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
            
            if (rb != null && rb.gameObject != gameObject) {
                // 3. Calculate direction from asteroid to this object
                Vector3 direction = transform.position - hitCollider.transform.position;
                float distance = direction.magnitude;

                if (distance > 0.1f) { // Prevent division by zero
                    // 4. Calculate Force (Inverse Square Law)
                    // Force = Intensity / (Distance * Distance)
                    float forceMagnitude = gravityIntensity / (distance * distance);
                    Vector3 force = direction.normalized * forceMagnitude;

                    // 5. Apply the pull
                    rb.AddForce(force);
                }
            }
        }
    }

    // This runs automatically in the Unity Editor
    private void OnDrawGizmos() {
        // Set the color of the sphere
        Gizmos.color = Color.cyan;
        
        // Calculate the same dynamic radius for the visual Gizmo
        float dynamicRadius = pullRadius * transform.localScale.x;

        // Draw the wireframe sphere at the Cube's position
        // Updated to use the dynamicRadius so the Cyan circle grows in the scene view
        Gizmos.DrawWireSphere(transform.position, dynamicRadius);
    }
}