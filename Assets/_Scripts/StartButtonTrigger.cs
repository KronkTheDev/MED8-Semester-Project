using UnityEngine;

public class StartButtonTrigger : MonoBehaviour
{
    [Header("Game Systems to Enable")]
    [SerializeField] private AsteroidSpawner asteroidSpawner;
    [SerializeField] private PlanetManager planetManager;
    [SerializeField] private GameObject planetObject;

    [Header("Menu Elements to Clean Up")]
    [SerializeField] private GameObject startText;
    [SerializeField] private GameObject startBText; // Text on the button
    [SerializeField] private GameObject blockA;
    [SerializeField] private GameObject blockB;

    private bool gameStarted = false;

    // Fail-Safe 1: This fires automatically for solid, physical collisions
    private void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision.gameObject);
    }

    // Fail-Safe 2: This fires automatically if colliders accidentally switch to triggers
    private void OnTriggerEnter(Collider other)
    {
        EvaluateCollision(other.gameObject);
    }

    // Consolidated method to read tags or names without duplicating code
    private void EvaluateCollision(GameObject hitObject)
    {
        if (gameStarted) return;

        // Check by tag or name instead of exact object instances to avoid SDK authority locks
        if (hitObject.CompareTag("ConditionA") || hitObject.name.Contains("A"))
        {
            Debug.Log("Condition A Selection Validated via tag/name match.");
            StartRegularGame();
        }
        else if (hitObject.CompareTag("ConditionB") || hitObject.name.Contains("B"))
        {
            Debug.Log("Condition B Selection Validated via tag/name match.");
            StartRegularGame();
        }
    }

    private void StartRegularGame()
    {
        gameStarted = true;

        // 1. Clean up all menu elements from the scene immediately
        if (startText != null) startText.SetActive(false);
        if (startBText != null) startBText.SetActive(false);
        if (blockA != null) Destroy(blockA);
        if (blockB != null) Destroy(blockB);

        // 2. Turn on the core gameplay components from before
        if (planetObject != null) planetObject.SetActive(true);
        if (planetManager != null) planetManager.enabled = true;
        
        if (asteroidSpawner != null) 
        {
            asteroidSpawner.enabled = true;
            // FIXED: Explicitly commands the spawner to kick off its InvokeRepeating loop now!
            asteroidSpawner.StartSpawningLoop(); 
        }

        Debug.Log("Gameplay loops successfully activated!");

        // 3. Destroy the button itself since the game has begun
        Destroy(gameObject);
    }
}