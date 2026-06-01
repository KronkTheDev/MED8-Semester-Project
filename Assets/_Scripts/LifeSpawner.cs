using UnityEngine;

public class LifeSpawner : MonoBehaviour {
    public GameObject[] lifePrefabs; // Add trees, houses, etc. here
    public float spawnRange = 30f;
    public float spawnInterval = 2f;
    
    private bool isSpawning = false;

    // This will be called by the PlanetFusion script
    public void StartLifePhase() {
        if (!isSpawning) {
            isSpawning = true;
            InvokeRepeating(nameof(SpawnLife), 0.5f, spawnInterval);
        }
    }

    void SpawnLife() {
        if (lifePrefabs.Length == 0) return;
        
        Vector3 randomPos = Random.insideUnitSphere * spawnRange;
        GameObject prefab = lifePrefabs[Random.Range(0, lifePrefabs.Length)];
        GameObject spawned = Instantiate(prefab, randomPos, Quaternion.identity);
        
        // Ensure these objects have the "Life" tag
        spawned.tag = "Life";
    }
}