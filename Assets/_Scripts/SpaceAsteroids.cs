using UnityEngine;
using Oculus.Interaction; 

public class Asteroid : MonoBehaviour {
    public int size = 3;

    [Header("Speed Settings")]
    public float minSpeed = 2f;
    public float maxSpeed = 20f;

    private void Start() {
        // Fix: Cast or look for the base PointableElement to listen to unified interaction events
        PointableElement pointable = GetComponent<PointableElement>();
        if (pointable != null) {
            pointable.WhenPointerEventRaised += (pointerEvent) => {
                // Triggers perfectly whenever any interactor (Direct Hand, Distance, or Ray) selects the object
                if (pointerEvent.Type == PointerEventType.Select) {
                    if (TelemetryLogger.Instance != null) {
                        TelemetryLogger.Instance.LogAsteroidGrab();
                    }
                }
            };
        }

        transform.localScale = 0.5f * size * Vector3.one;

        Rigidbody rb = GetComponent<Rigidbody>();
        
        if (rb != null) {
            float randomBase = Random.Range(minSpeed, maxSpeed);
            float uniqueNoise = Random.value * 0.01f; 
            float finalSpeed = (randomBase + uniqueNoise) / (size * 0.5f);

            Vector3 direction = Random.onUnitSphere;

            rb.AddForce(direction * finalSpeed, ForceMode.Impulse);

            rb.angularVelocity = Random.insideUnitSphere * Random.Range(1f, 3f);
            
            Debug.Log($"Spawned {gameObject.name} with a unique random speed: {finalSpeed}");
        }
    }
}