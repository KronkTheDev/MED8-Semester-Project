using UnityEngine;
using TMPro; // Needed for TextMeshPro
using System.Collections;

public class StartMessage : MonoBehaviour {
    public float displayDuration = 3.0f; // How long it stays fully visible
    public float fadeDuration = 2.0f;    // How long it takes to vanish

    private TextMeshProUGUI textMesh;

    void Start() {
        textMesh = GetComponent<TextMeshProUGUI>();
        // Start the timer to hide the message
        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine() {
        // 1. Wait for a few seconds
        yield return new WaitForSeconds(displayDuration);

        // 2. Gradually fade the alpha (transparency) to 0
        float elapsed = 0;
        Color startColor = textMesh.color;

        while (elapsed < fadeDuration) {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        // 3. Deactivate the object entirely when done
        gameObject.SetActive(false);
    }
}