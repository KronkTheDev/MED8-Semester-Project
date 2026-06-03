using UnityEngine;

public class StartButtonTrigger : MonoBehaviour
{
    [Header("Game Systems to Enable")]
    [SerializeField] private AsteroidSpawner asteroidSpawner;
    [SerializeField] private PlanetManager planetManager;
    [SerializeField] private GameObject planetObject;
    [SerializeField] private GameObject stage1Text; 

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
            Debug.Log("Condition A Selected (Visual Cues Enabled).");
            // False means cues function as normal
            if (planetManager != null) planetManager.hideVisualCues = false; 
            StartRegularGame(false); 
        }
        else if (hitObject.CompareTag("ConditionB") || hitObject.name.Contains("B"))
        {
            Debug.Log("Condition B Selected (Visual Cues Disabled - Blind Route).");
            // True forces all instructions and scale text to stay hidden
            if (planetManager != null) planetManager.hideVisualCues = true; 
            StartRegularGame(true); 
        }
    }

    private void StartRegularGame(bool isBlindRoute)
    {
        gameStarted = true;

        // 1. Clean up menu objects
        if (startText != null) startText.SetActive(false);
        if (startBText != null) startBText.SetActive(false);
        if (blockA != null) Destroy(blockA);
        if (blockB != null) Destroy(blockB);

        // 2. Wake up target game components
        if (planetObject != null) planetObject.SetActive(true);
        if (planetManager != null) planetManager.enabled = true;
        
        // FIXED: Only display the initial "Stage 1" instruction text if it's NOT the blind route
        if (stage1Text != null && !isBlindRoute) {
            stage1Text.SetActive(true); 
        }
        
        if (asteroidSpawner != null) 
        {
            asteroidSpawner.enabled = true;
            asteroidSpawner.StartSpawningLoop(); 
        }

        Debug.Log($"Gameplay activated. Blind Mode Status: {isBlindRoute}");

        // 3. Complete sequence by removing selection interface
        Destroy(gameObject);
    }
}