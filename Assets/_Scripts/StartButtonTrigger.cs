using UnityEngine;

public class StartButtonTrigger : MonoBehaviour
{
    [Header("Game Systems to Enable")]
    [SerializeField] private AsteroidSpawner asteroidSpawner;
    [SerializeField] private PlanetManager planetManager;
    [SerializeField] private GameObject planetObject;
    [SerializeField] private GameObject stage1Text; // ADDED: Slot for your Stage 1 text

    [Header("Menu Elements to Clean Up")]
    [SerializeField] private GameObject startText;
    [SerializeField] private GameObject startBText; 
    [SerializeField] private GameObject blockA;
    [SerializeField] private GameObject blockB;

    private bool gameStarted = false;

    private void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        EvaluateCollision(other.gameObject);
    }

    private void EvaluateCollision(GameObject hitObject)
    {
        if (gameStarted) return;

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

        // 2. Turn on the core gameplay components and Stage 1 Text
        if (planetObject != null) planetObject.SetActive(true);
        if (planetManager != null) planetManager.enabled = true;
        
        // FIXED: This wakes up your Stage 1 text the exact second the game starts!
        if (stage1Text != null) stage1Text.SetActive(true); 
        
        if (asteroidSpawner != null) 
        {
            asteroidSpawner.enabled = true;
            asteroidSpawner.StartSpawningLoop(); 
        }

        Debug.Log("Gameplay loops successfully activated!");

        // 3. Destroy the button itself since the game has begun
        Destroy(gameObject);
    }
}