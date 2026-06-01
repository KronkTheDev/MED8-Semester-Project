using UnityEngine;

public class Asteroid : MonoBehaviour {
    public int size = 3;

    [Header("Speed Settings")]
    public float minSpeed = 2f;
    public float maxSpeed = 20f;

    private void Start() {
        // 1. Scale based on the size
        transform.localScale = 0.5f * size * Vector3.one;

        Rigidbody rb = GetComponent<Rigidbody>();
        
        if (rb != null) {
            // 2. Pick a completely random speed within your range
            // We use (float)Random.value to add a tiny unique decimal "noise"
            float randomBase = Random.Range(minSpeed, maxSpeed);
            float uniqueNoise = Random.value * 0.01f; 
            float finalSpeed = (randomBase + uniqueNoise) / (size * 0.5f);

            // 3. Pick a random 3D direction
            Vector3 direction = Random.onUnitSphere;

            // 4. Apply the force
            rb.AddForce(direction * finalSpeed, ForceMode.Impulse);

            // 5. Give it a random 3D spin (Tumble)
            rb.angularVelocity = Random.insideUnitSphere * Random.Range(1f, 3f);
            
            Debug.Log($"Spawned {gameObject.name} with a unique random speed: {finalSpeed}");
        }
    }
}