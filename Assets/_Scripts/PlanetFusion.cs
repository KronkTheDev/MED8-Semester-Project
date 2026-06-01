using UnityEngine;

public class PlanetFusion : MonoBehaviour {
    [Header("Growth Settings")]
    public float growthIncrement = 0.05f; // How much the planet grows per hit
    public float maxScale = 10f;          // Limit how big the planet can get

    [Header("UI References")]
    public GameObject Stage2text; // Drag the 'Stage2text' object here

    private bool hasReachedMax = false;
    private void OnCollisionEnter(Collision collision) {
        // 1. Check if the object hitting us is an Asteroid
        // Make sure your Asteroid Prefab has the Tag "Asteroid" set in the Inspector!
        if (collision.gameObject.CompareTag("Asteroid")) {
            GrowPlanet();
            
            // 2. Destroy the asteroid immediately so it "disappears" into the planet
            Destroy(collision.gameObject);
        }
    }
    
    void GrowPlanet() {
        if (transform.localScale.x < maxScale) {
            transform.localScale += Vector3.one * growthIncrement;
        }
        // Check if we just hit the limit
        else if (!hasReachedMax) {
            TriggerStage2State();
        }
    }

    public LifeSpawner lifeSpawner;
    void TriggerStage2State() {
        hasReachedMax = true;
        
        if (Stage2text != null) {
            Stage2text.SetActive(true); // This makes the hidden text appear!
                hasReachedMax = true;
                Stage2text.SetActive(true);
                lifeSpawner.StartLifePhase(); // START THE LIFE SPAWNING
        }

        Debug.Log("Planet is full! Phase 2: Bring surface to life.");
    }
}