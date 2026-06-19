using UnityEngine;
using System.Collections;
using TMPro;

public class PlanetManager : MonoBehaviour
{
    [Header("Condition Selector Switch")]
    public bool hideVisualCues = false; 

    [Header("Phase 1: Scale Counter UI")]
    public GameObject scaleCounterCanvas;
    public TextMeshProUGUI scaleCounterText;
    private float maxScaleGoal = 200f; 
    private bool scaleCounterActivated = false;

    [Header("Phase 2: Blooming (Life Counter UI)")]
    public int lifeGoal = 10;
    private int currentLife = 0;
    public GameObject lifeCounterCanvas;
    public TextMeshProUGUI lifeCounterText;
    public GameObject Stage2text; 
    public GameObject spawner; 
    public GameObject Stage3text; 

    [Header("Phase 3: Survival (Timer UI)")]
    public float survivalDuration = 20f;
    public float scaleLossPerHit = 10f;
    public float loseThresholdScale = 30f;
    public float asteroidSpeedInPhase2 = 15f; 
    public GameObject timerCanvas;
    public TextMeshProUGUI timerText;
    private float timeRemaining;

    [Header("End Game UI")]
    public GameObject winCanvas;
    public GameObject loseCanvas;
    public GameObject blackOverlay; 

    // FIXED: Made public so the Spawner script can read the actual game state directly!
    public bool isPhase2 { get; private set; } = false;
    public bool isPhase3 { get; private set; } = false;
    private bool gameEnded = false;

    void Start() {
        if (scaleCounterCanvas != null) scaleCounterCanvas.SetActive(false);
        if (lifeCounterCanvas != null) lifeCounterCanvas.SetActive(false);
        if (timerCanvas != null) timerCanvas.SetActive(false);

        if (Stage2text != null) Stage2text.SetActive(false);
        Stage3text.SetActive(false);
        winCanvas.SetActive(false);
        loseCanvas.SetActive(false);
        blackOverlay.SetActive(false);

        timeRemaining = survivalDuration;
    }

    void Update() {
        if (gameEnded) return;

        if (!hideVisualCues) {
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
    }

    void OnCollisionEnter(Collision collision) {
        if (gameEnded) return;

        if (collision.gameObject.CompareTag("Asteroid")) {
            if (!isPhase2) {
                TelemetryLogger logger = GetComponent<TelemetryLogger>();
                if (logger != null) logger.LogCollision();

                if (!scaleCounterActivated) {
                    scaleCounterActivated = true;
                    if (scaleCounterCanvas != null && !hideVisualCues) {
                        scaleCounterCanvas.SetActive(true);
                    }
                }

                transform.localScale += Vector3.one * 2.0f; 
                Destroy(collision.gameObject);

                if (transform.localScale.x >= maxScaleGoal) {
                    TransitionToPhase2();
                }
            }
            else if (isPhase3) {
                transform.localScale -= Vector3.one * scaleLossPerHit;
                Destroy(collision.gameObject);

                if (transform.localScale.x <= loseThresholdScale) {
                    EndGame(false);
                }
            }
        } 
        else if (collision.gameObject.CompareTag("Life") && isPhase2 && !isPhase3) {
            currentLife++;
            
            collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            collision.gameObject.transform.SetParent(this.transform);

            if (!hideVisualCues) UpdateLifeDisplay();

            if (currentLife >= lifeGoal) {
                StartCoroutine(TransitionToPhase3());
            }
        }
    }

    void TransitionToPhase2() {
        TelemetryLogger logger = GetComponent<TelemetryLogger>();
        if (logger != null) logger.SaveStage1Telemetry();

        isPhase2 = true;
        
        // FIXED: Structural canvas adjustments pulled out of the hideVisualCues gate
        if (scaleCounterCanvas != null) scaleCounterCanvas.SetActive(false);

        if (!hideVisualCues) {
            if (lifeCounterCanvas != null) lifeCounterCanvas.SetActive(true);
            if (Stage2text != null) Stage2text.SetActive(true); 
            UpdateLifeDisplay();
        }
    }

    IEnumerator TransitionToPhase3() {
        if (Stage2text != null) Stage2text.SetActive(false);

        spawner.SetActive(false); 
        
        GameObject[] leftovers = GameObject.FindGameObjectsWithTag("Asteroid");
        foreach (GameObject o in leftovers) {
            if (o.transform.parent != this.transform) Destroy(o);
        }

        if (lifeCounterCanvas != null) lifeCounterCanvas.SetActive(false);

        if (!hideVisualCues) {
            if (timerCanvas != null) timerCanvas.SetActive(true);
            if (Stage3text != null) Stage3text.SetActive(true); 
        }

        AsteroidSpawner sScript = spawner.GetComponent<AsteroidSpawner>();
        if(sScript != null) sScript.asteroidSpeed = asteroidSpeedInPhase2;
        
        spawner.SetActive(true); 

        yield return new WaitForSeconds(5f); 

        if (!hideVisualCues && Stage3text != null) Stage3text.SetActive(false);
        isPhase3 = true; 
        
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
            timerText.text = $"Time Left: {Mathf.Max(0, timeRemaining):F0}s";
        }
    }

    void EndGame(bool won) {
        gameEnded = true;
        spawner.SetActive(false);
        
        if (blackOverlay != null) blackOverlay.SetActive(true); 

        if (timerCanvas != null) timerCanvas.SetActive(false);

        if (!hideVisualCues) {
            if (won && winCanvas != null) winCanvas.SetActive(true);
            else if (!won && loseCanvas != null) loseCanvas.SetActive(true);
        }

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