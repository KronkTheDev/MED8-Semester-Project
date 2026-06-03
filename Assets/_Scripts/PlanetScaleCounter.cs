using UnityEngine;
using TMPro;

public class PlanetScaleCounter : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject counterCanvas;
    [SerializeField] private TextMeshProUGUI counterText;

    private float maxScale;
    private bool counterActivated = false;

    void Start()
    {
        // Capture the planet's starting size as the maximum scale threshold
        maxScale = transform.localScale.x;

        // Ensure it starts hidden
        if (counterCanvas != null) counterCanvas.SetActive(false);
    }

    // This will be called by your impact code when the first asteroid hits
    public void ActivateCounter()
    {
        if (counterActivated) return;
        counterActivated = true;

        if (counterCanvas != null) counterCanvas.SetActive(true);
        UpdateDisplay();
    }

    void Update()
    {
        // Keep the display updated in real-time as the planet scales
        if (counterActivated)
        {
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (counterText == null) return;

        // Reads the current X scale of the planet
        float currentScale = transform.localScale.x;

        // Formats the numbers to 1 decimal place (e.g., "3.5 / 5.0")
        counterText.text = $"{currentScale:F1} / {maxScale:F1}";
    }
}