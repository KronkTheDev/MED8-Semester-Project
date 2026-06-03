using UnityEngine;

public class AsteroidSpawner : MonoBehaviour {
    [Header("References")]
    public GameObject asteroidPrefab;
    public Transform planetTransform; 
    [Tooltip("The 'Protect me' Canvas. When this is active, asteroids attack.")]
    public GameObject triggerCanvas; 

    [Header("Spawn Settings")]
    public float secondsBetweenSpawns = 3.0f; 
    public float spawnRange = 10.0f;
    [Tooltip("Initial speed. PlanetManager will increase this during Phase 3.")]
    public float asteroidSpeed = 5.0f; 

    private void Start() {
        // Leave this empty now! The script will wait until StartSpawningLoop() is called.
    }

    /// <summary>
    /// Public function called by the StartButtonTrigger when a condition block is chosen.
    /// </summary>
    public void StartSpawningLoop() {
        // Begins the spawning loop safely *after* a selection is made
        InvokeRepeating(nameof(SpawnOnlyOne), 0f, secondsBetweenSpawns);
        Debug.Log("Asteroid Spawner loop has been officially activated!");
    }

    private void SpawnOnlyOne() {
        if (asteroidPrefab == null || planetTransform == null || triggerCanvas == null) return;

        // THE SWITCH: Only attack if the Canvas is visible on screen
        bool isAttacking = triggerCanvas.activeInHierarchy;

        Vector3 randomPos = transform.position + new Vector3(
            Random.Range(-spawnRange, spawnRange),
            Random.Range(-spawnRange, spawnRange),
            Random.Range(-spawnRange, spawnRange)
        );

        GameObject newAsteroid = Instantiate(asteroidPrefab, randomPos, Quaternion.identity);
        Rigidbody rb = newAsteroid.GetComponent<Rigidbody>();

        if (rb != null) {
            if (isAttacking) {
                // PHASE 2: Aim and fire toward the planet
                Vector3 direction = (planetTransform.position - randomPos).normalized;
                rb.linearVelocity = direction * asteroidSpeed; 
            } else {
                // PHASE 1: Float in place (Normal behavior)
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Random.insideUnitSphere * 2f; 
            }
        }
    }
}