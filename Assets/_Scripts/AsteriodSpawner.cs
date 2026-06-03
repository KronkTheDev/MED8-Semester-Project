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

    private bool isLoopRunning = false;

    private void Start() {
        // Handled cleanly by OnEnable and StartSpawningLoop
    }

    // FIXED: This automatically runs the exact moment PlanetManager calls spawner.SetActive(true) in Phase 3!
    private void OnEnable() {
        // Only run this automatically if the game has already been kicked off by the main menu button
        if (isLoopRunning) {
            CancelInvoke(nameof(SpawnOnlyOne)); 
            InvokeRepeating(nameof(SpawnOnlyOne), 0.5f, secondsBetweenSpawns);
            Debug.Log("Phase 3: Asteroid Spawner re-awakened. Hostile loop restarted!");
        }
    }

    /// <summary>
    /// Public function called by the StartButtonTrigger when a condition block is chosen.
    /// </summary>
    public void StartSpawningLoop() {
        if (isLoopRunning) return;
        
        isLoopRunning = true;
        CancelInvoke(nameof(SpawnOnlyOne)); 
        InvokeRepeating(nameof(SpawnOnlyOne), 0f, secondsBetweenSpawns);
        Debug.Log("Asteroid Spawner loop has been officially activated!");
    }

    // Automatically stops and purges all scheduled Invoke timers 
    // the exact millisecond PlanetManager calls SetActive(false)
    private void OnDisable() {
        CancelInvoke(nameof(SpawnOnlyOne));
        Debug.Log("Spawner Disabled: Internal timers completely wiped clean.");
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
                // PHASE 3: Aim and fire toward the planet
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