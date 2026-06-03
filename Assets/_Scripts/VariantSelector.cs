using UnityEngine;

public class VariantSelector : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject startText;

    [Header("A/B Selection Blocks")]
    [SerializeField] private GameObject blockA;
    [SerializeField] private GameObject blockB;

    [Header("Game Systems to Enable")]
    [SerializeField] private AsteroidSpawner asteroidSpawner;
    [SerializeField] private PlanetManager planetManager;
    [SerializeField] private GameObject planetObject;

    private bool selectionMade = false;

    void Start()
    {
        selectionMade = false;

        // Ensure menu elements are active
        if (startText != null) startText.SetActive(true);
        if (blockA != null) blockA.SetActive(true);
        if (blockB != null) blockB.SetActive(true);

        // Completely disable the gameplay systems so they don't run in the background
        if (asteroidSpawner != null) asteroidSpawner.enabled = false;
        if (planetManager != null) planetManager.enabled = false;
        if (planetObject != null) planetObject.SetActive(false);
    }

    // Call this on Block A's "Select Entered" event
    public void SelectConditionA()
    {
        if (selectionMade) return;
        selectionMade = true;
        
        Debug.Log("Condition A Selected!");
        // If your PlanetManager has a variable for the A/B switch, set it here:
        // planetManager.isConditionA = true; 

        StartRegularGame();
    }

    // Call this on Block B's "Select Entered" event
    public void SelectConditionB()
    {
        if (selectionMade) return;
        selectionMade = true;

        Debug.Log("Condition B Selected!");
        // planetManager.isConditionA = false;

        StartRegularGame();
    }

    private void StartRegularGame()
    {
        // 1. Clean up menu objects immediately so they don't interfere with physics
        if (startText != null) startText.SetActive(false);
        if (blockA != null) Destroy(blockA);
        if (blockB != null) Destroy(blockB);

        // 2. Wake up your game systems
        if (planetObject != null) planetObject.SetActive(true);
        if (planetManager != null) planetManager.enabled = true;
        if (asteroidSpawner != null) asteroidSpawner.enabled = true;

        Debug.Log("Gameplay loops successfully activated!");

        // 3. Remove the selector manager since its job is complete
        Destroy(gameObject);
    }
}