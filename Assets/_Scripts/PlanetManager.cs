using UnityEngine;
using System.Collections;
using TMPro; // Ensure TextMeshPro is included for UI handling

public class PlanetManager : MonoBehaviour
{
    [Header("Phase 1: Scale Counter UI")]
    [Tooltip("The Canvas/Text used to display 'Current Scale / Max Scale'.")]
    public GameObject scaleCounterCanvas;
    public TextMeshProUGUI scaleCounterText;
    private float maxScaleGoal = 200f; // Adjust this value to match your desired max scale limit
    private bool scaleCounterActivated = false;

    [Header("Phase 2: Blooming (Life Counter UI)")]
    public int lifeGoal = 10;
    private int currentLife = 0;
    [Tooltip("The Canvas/Text used to display 'Current Life / Target Life'.")]
    public GameObject lifeCounterCanvas;
    public TextMeshProUGUI lifeCounterText;
    public GameObject spawner; 
    public GameObject Stage3text; 

    [Header("Phase 3: Survival (Timer UI)")]
    public float survivalDuration = 20f;
    public float scaleLossPerHit = 10f;
    public float loseThresholdScale = 30f;
    public float asteroidSpeedInPhase2 = 15f; 
    [Tooltip("The Canvas/Text used to display the remaining seconds.")]
    public GameObject timerCanvas;
    public TextMeshProUGUI timerText;
    private float timeRemaining;

    [Header("End Game UI")]
    public GameObject winCanvas;
    public GameObject loseCanvas;
    public GameObject blackOverlay; // A black UI Panel to cover the VR view

    private bool isPhase2 = false;
    private bool isPhase3 = false;
    private bool gameEnded = false;

    void Start() {
        // Ensure ALL UI elements start hidden or in correct default state
        if (scaleCounterCanvas != null) scaleCounterCanvas.SetActive(false);
        if (lifeCounterCanvas != null) lifeCounterCanvas.SetActive(false);
        if (timerCanvas != null) timerCanvas.SetActive(false);

        Stage3text.SetActive(false);
        winCanvas.SetActive(false);
        loseCanvas.SetActive(false);
        blackOverlay.SetActive(false);

        timeRemaining = survivalDuration;
    }

    void Update() {
        if (gameEnded) return;

        // Dynamic Display updates based on current phase
        if (scaleCounterActivated && !isPhase2) {
            UpdateScaleDisplay();
        }
        else if (isPhase2 && !isPhase3) {
            UpdateLifeDisplay();
        }
        else if (isPhase3) {
            UpdateTimerDisplay();
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (gameEnded) return;

        // === PHASE 1 & 3: Asteroid Interactions ===
        if (collision.gameObject.CompareTag("Asteroid")) {
            
            // IF PHASE 1 (Not Phase 2 yet): Asteroids grow the planet
            if (!isPhase2) {
                // Wake up the scale counter on the very first hit
                if (!scaleCounterActivated) {
                    scaleCounterActivated = true;
                    if (scaleCounterCanvas != null) scaleCounterCanvas.SetActive(true);
                }

                // Make the planet grow! (Adjust Vector3 multiplier if growing too fast)
                transform.localScale += Vector3.one * 2.0f; 
                Destroy(collision.gameObject);

                // Transition criteria: Once planet hits or passes max scale target, move to Phase 2
                if (transform.localScale.x >= maxScaleGoal) {
                    TransitionToPhase2();
                }
            }
            // IF PHASE 3: Asteroids attack and shrink the planet
            else if (isPhase3) {
                transform.localScale -= Vector3.one * scaleLossPerHit;
                Destroy(collision.gameObject);

                if (transform.localScale.x <= loseThresholdScale) {
                    EndGame(false);
                }
            }
        } 
        
        // === PHASE 2: Collecting Life Nodes ===
        else if (collision.gameObject.CompareTag("Life") && isPhase2 && !isPhase3) {
            currentLife++;
            
            // Stick life to planet
            collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            collision.gameObject.transform.SetParent(this.transform);

            // Update display immediately on collection
            UpdateLifeDisplay();

            // Transition criteria: Once target goals are met, move to Phase 3
            if (currentLife >= lifeGoal) {
                StartCoroutine(TransitionToPhase3());
            }
        }
    }

    void TransitionToPhase2() {
        isPhase2 = true;
        
        // Swap UI Panels cleanly
        if (scaleCounterCanvas != null) scaleCounterCanvas.SetActive(false);
        if (lifeCounterCanvas != null) lifeCounterCanvas.SetActive(true);
        
        UpdateLifeDisplay();
        Debug.Log("Phase 1 Complete. Phase 2 (Life Collection) initiated!");
    }

    IEnumerator TransitionToPhase3() {
        // Pause spawning briefly
        spawner.SetActive(false); 
        
        // Clear leftover floating asteroids
        GameObject[] leftovers = GameObject.FindGameObjectsWithTag("Asteroid");
        foreach (GameObject o in leftovers) {
            if (o.transform.parent != this.transform) Destroy(o);
        }

        // Swap UI Panels from Life tracking over to the Countdown Timer
        if (lifeCounterCanvas != null) lifeCounterCanvas.SetActive(false);
        if (timerCanvas != null) timerCanvas.SetActive(true);

        Stage3text.SetActive(true); 

        // Update spawner configurations
        AsteroidSpawner sScript = spawner.GetComponent<AsteroidSpawner>();
        if(sScript != null) sScript.asteroidSpeed = asteroidSpeedInPhase2;
        
        spawner.SetActive(true); 

        yield return new WaitForSeconds(5f); 

        Stage3text.SetActive(false);
        isPhase3 = true; // Turn on hostile damage tracking
        
        StartCoroutine(SurvivalCountdown());
    }

    IEnumerator SurvivalCountdown() {
        while (timeRemaining > 0) {
            yield return new WaitForSeconds(1f);
            timeRemaining--;
            if (timeRemaining <= 0) break;
        }

        if (!gameEnded) EndGame(true);
    }

    // === UI Text String Formatting Methods ===

    private void UpdateScaleDisplay() {
        if (scaleCounterText != null) {
            scaleCounterText.text = $"Scale: {transform.localScale.x:F1} / {maxScaleGoal:F1}";
        }
    }

    private void UpdateLifeDisplay() {
        if (lifeCounterText != null) {
            lifeCounterText.text = $"Life: {currentLife} / {lifeGoal}";
        }
    }

    private void UpdateTimerDisplay() {
        if (timerText != null) {
            // Displays cleanly as simple integer seconds remaining (e.g., "Time: 18s")
            timerText.text = $"Time Left: {Mathf.Max(0, timeRemaining):F0}s";
        }
    }

    void EndGame(bool won) {
        gameEnded = true;
        spawner.SetActive(false);
        blackOverlay.SetActive(true); 

        if (timerCanvas != null) timerCanvas.SetActive(false);

        if (won) winCanvas.SetActive(true);
        else loseCanvas.SetActive(true);

        Invoke("QuitGame", 5f);
    }

    void QuitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}