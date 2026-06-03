using UnityEngine;

public class PlanetFusion : MonoBehaviour {
    [Header("Growth Settings")]
    public float growthIncrement = 0.05f; 
    public float maxScale = 10f;          

    [Header("UI References")]
    public GameObject Stage2text; 

    public LifeSpawner lifeSpawner;
    public PlanetManager planetManager;

    private bool hasReachedMax = false;

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Asteroid")) {
            GrowPlanet();
            Destroy(collision.gameObject);
        }
    }
    
    void GrowPlanet() {
        if (transform.localScale.x < maxScale) {
            transform.localScale += Vector3.one * growthIncrement;
            
            if (transform.localScale.x >= maxScale && !hasReachedMax) {
                TriggerStage2State();
            }
        }
    }

    void TriggerStage2State() {
        hasReachedMax = true;
        
        if (planetManager != null && planetManager.hideVisualCues) {
            if (Stage2text != null) Stage2text.SetActive(false);
        } else {
            if (Stage2text != null) Stage2text.SetActive(true);
        }

        if (lifeSpawner != null) {
            lifeSpawner.StartLifePhase();
        }

        Debug.Log("Planet is full! Phase 2: Bring surface to life.");
    }
}