using UnityEngine;

public class MagneticStick : MonoBehaviour {
    public bool isGrabbed = false;
    private Transform planet;

    void Start() {
        planet = GameObject.FindWithTag("Planet").transform; // Ensure your planet is tagged "Planet"
    }

    void Update() {
        if (!isGrabbed && planet != null) {
            // "Magnetic" pull: Look at the center of the planet
            Vector3 dirToPlanet = (planet.position - transform.position).normalized;
            
            // Align 'Up' with the surface normal
            transform.rotation = Quaternion.FromToRotation(Vector3.up, -dirToPlanet);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Planet")) {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) {
                rb.isKinematic = true; // Stop moving
                transform.SetParent(collision.transform); // Stick to surface
            }
        }
    }
}