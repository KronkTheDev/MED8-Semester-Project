using UnityEngine;

public class LifeSpawner : MonoBehaviour {
    [Header("Prefabs & Range")]
    public GameObject[] lifePrefabs; // Trees, houses, etc.
    public float spawnRange = 30f;
    public float spawnInterval = 2f;
    
    private bool isSpawning = false;

    /// <summary>
    /// Called by PlanetManager when Phase 2 officially begins.
    /// </summary>
    public void StartLifePhase() {
        if (!isSpawning) {
            isSpawning = true;
            // Clear any old accidental cycles first just to be perfectly safe
            CancelInvoke(nameof(SpawnLife)); 
            InvokeRepeating(nameof(SpawnLife), 0.5f, spawnInterval);
            Debug.Log("Life Spawner loop has been officially activated for Phase 2!");
        }
    }

    // CRITICAL FIX: Automatically stops and purges all scheduled Invoke timers 
    // the exact millisecond PlanetManager tells Phase 2 to end.
    private void OnDisable() {
        CancelInvoke(nameof(SpawnLife));
        isSpawning = false;
        Debug.Log("Life Spawner Disabled: Internal timers completely wiped clean.");
    }

    void SpawnLife() {
        if (lifePrefabs.Length == 0) return;
        
        Vector3 randomPos = Random.insideUnitSphere * spawnRange;
        GameObject prefab = lifePrefabs[Random.Range(0, lifePrefabs.Length)];
        GameObject spawned = Instantiate(prefab, randomPos, Quaternion.identity);
        
        // Ensure these objects explicitly carry the "Life" tag for collision detection
        spawned.tag = "Life";
    }
}