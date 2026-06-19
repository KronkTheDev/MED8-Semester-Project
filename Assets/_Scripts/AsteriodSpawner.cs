using UnityEngine;
using System.Collections.Generic;

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
    private PlanetManager planetManager;
    
    // Internal list to hold all your discovered life prefabs dynamically
    private List<GameObject> discoveredLifePrefabs = new List<GameObject>();

    private void Start() {
        planetManager = FindFirstObjectByType<PlanetManager>();
        LoadLifePrefabsByTag();
    }

    // Automatically scans the Resources folder for any prefab carrying the "Life" tag
    private void LoadLifePrefabsByTag() {
        GameObject[] allResources = Resources.LoadAll<GameObject>("");
        
        foreach (GameObject obj in allResources) {
            if (obj.CompareTag("Life")) {
                discoveredLifePrefabs.Add(obj);
            }
        }
        
        Debug.Log($"Spawner Setup: Automatically found and loaded {discoveredLifePrefabs.Count} prefabs tagged 'Life' from Resources!");
    }

    private void OnEnable() {
        if (isLoopRunning) {
            CancelInvoke(nameof(SpawnOnlyOne)); 
            InvokeRepeating(nameof(SpawnOnlyOne), 0.5f, secondsBetweenSpawns);
            Debug.Log("Phase 3: Spawner re-awakened. Hostile loop restarted!");
        }
    }

    public void StartSpawningLoop() {
        if (isLoopRunning) return;
        
        isLoopRunning = true;
        CancelInvoke(nameof(SpawnOnlyOne)); 
        InvokeRepeating(nameof(SpawnOnlyOne), 0f, secondsBetweenSpawns);
        Debug.Log("Spawner loop has been officially activated!");
    }

    private void OnDisable() {
        CancelInvoke(nameof(SpawnOnlyOne));
        Debug.Log("Spawner Disabled: Internal timers completely wiped clean.");
    }

    private void SpawnOnlyOne() {
        if (asteroidPrefab == null || planetTransform == null) return;
        if (planetManager == null) planetManager = FindFirstObjectByType<PlanetManager>();

        GameObject prefabToSpawn = asteroidPrefab;
        
        // DYNAMIC SELECTION VIA TAG LIST:
        if (planetManager != null && planetManager.isPhase2 && !planetManager.isPhase3) {
            if (discoveredLifePrefabs.Count > 0) {
                int randomIndex = Random.Range(0, discoveredLifePrefabs.Count);
                prefabToSpawn = discoveredLifePrefabs[randomIndex];
            }
        }

        bool isAttacking = (planetManager != null) ? planetManager.isPhase3 : triggerCanvas.activeInHierarchy;

        Vector3 randomPos = transform.position + new Vector3(
            Random.Range(-spawnRange, spawnRange),
            Random.Range(-spawnRange, spawnRange),
            Random.Range(-spawnRange, spawnRange)
        );

        GameObject newObject = Instantiate(prefabToSpawn, randomPos, Quaternion.identity);
        Rigidbody rb = newObject.GetComponent<Rigidbody>();

        if (rb != null) {
            if (isAttacking) {
                Vector3 direction = (planetTransform.position - randomPos).normalized;
                rb.linearVelocity = direction * asteroidSpeed; 
            } else {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Random.insideUnitSphere * 2f; 
            }
        }
    }
}