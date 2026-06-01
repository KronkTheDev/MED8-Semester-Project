 using UnityEngine;
using System.Collections;

public class PlanetManager : MonoBehaviour
{
    [Header("Phase 2: Blooming")]
    public int lifeGoal = 10;
    private int currentLife = 0;
    public GameObject spawner; 
    public GameObject Stage3text; 

    [Header("Phase 3: Survival")]
    public float survivalDuration = 20f;
    public float scaleLossPerHit = 10f;
    public float loseThresholdScale = 30f;
    public float asteroidSpeedInPhase2 = 15f; 

    [Header("End Game UI")]
    public GameObject winCanvas;
    public GameObject loseCanvas;
    public GameObject blackOverlay; // A black UI Panel to cover the VR view

    private bool isPhase2 = false;
    private bool gameEnded = false;

    void Start() {
        // Ensure UI is in correct starting state
        Stage3text.SetActive(false);
        winCanvas.SetActive(false);
        loseCanvas.SetActive(false);
        blackOverlay.SetActive(false);
    }

    void OnCollisionEnter(Collision collision) {
    if (gameEnded) return;

    // PHASE 2
    if (collision.gameObject.CompareTag("Life") && !isPhase2) {
        currentLife++;
        
        // Stick life to planet
        collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        collision.gameObject.transform.SetParent(this.transform);

        // ONLY start Phase 3 if the Life Goal is reached
        if (currentLife >= lifeGoal) {
            StartCoroutine(TransitionToPhase3());
        }
    } 
    // PHASE 3:
    else if (collision.gameObject.CompareTag("Asteroid") && isPhase2) {
        // Reduce planet scale
        transform.localScale -= Vector3.one * scaleLossPerHit;
        Destroy(collision.gameObject);

        // Check if lost
        if (transform.localScale.x <= loseThresholdScale) {
            EndGame(false);
        }
    }
}

    IEnumerator TransitionToPhase3() {
        // Pause spawning briefly
        spawner.SetActive(false); 
        
        // Remove floating asteroids that aren't attached to planet
        GameObject[] leftovers = GameObject.FindGameObjectsWithTag("Asteroid");
        foreach (GameObject o in leftovers) {
            if (o.transform.parent != this.transform) Destroy(o);
        }

        // TRIGGER THE SWITCH: Show text (This activates 'isAttacking' in Spawner)
        Stage3text.SetActive(true); 

        // Update spawner speed and restart it
        AsteroidSpawner sScript = spawner.GetComponent<AsteroidSpawner>();
        if(sScript != null) sScript.asteroidSpeed = asteroidSpeedInPhase2;
        
        spawner.SetActive(true); 

        // Wait 5 seconds while text is visible and asteroids are attacking
        yield return new WaitForSeconds(5f); 

        Stage3text.SetActive(false);
        isPhase2 = true; // Permanent attack mode even if text is gone
        
        StartCoroutine(SurvivalCountdown());
    }

    IEnumerator SurvivalCountdown() {
        yield return new WaitForSeconds(survivalDuration);
        if (!gameEnded) EndGame(true);
    }

    void EndGame(bool won) {
        gameEnded = true;
        spawner.SetActive(false);
        blackOverlay.SetActive(true); // Screen goes pitch black

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