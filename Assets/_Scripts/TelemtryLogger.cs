using UnityEngine;
using System.IO;

public class TelemetryLogger : MonoBehaviour
{
    public static TelemetryLogger Instance;

    [Header("Setup")]
    public string participantID = "P1";
    public string groupName = "Group A";

    private int asteroidsGrabbed = 0;
    private int totalCollisions = 0;
    private float startTime;
    private bool stage1Active = true;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        startTime = Time.time;
    }

    public void LogAsteroidGrab()
    {
        if (stage1Active)
        {
            asteroidsGrabbed++;
            Debug.Log($"Telemetry Grab Registered! Total: {asteroidsGrabbed}");
        }
    }

    public void LogCollision()
    {
        if (stage1Active) totalCollisions++;
    }

    public void SaveStage1Telemetry()
    {
        if (!stage1Active) return;
        stage1Active = false;

        float totalPlayTimeSeconds = Time.time - startTime;
        
        int minutes = Mathf.FloorToInt(totalPlayTimeSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalPlayTimeSeconds % 60f);
        string formattedTime = $"{minutes}m {seconds:D2}s";

        // FIXED: Dynamically calculate accuracy and clamp it to 100% maximum
        float handAccuracy = 0f;
        if (asteroidsGrabbed > 0)
        {
            // If total collisions are lower than grabbed (e.g. dropped items), accuracy is less than 100%
            // If collisions match or exceed grabs due to passive help, it caps perfectly at 100%
            handAccuracy = Mathf.Min(100f, ((float)totalCollisions / (float)asteroidsGrabbed) * 100f);
        }

        string filePath = Path.Combine(Application.persistentDataPath, "TelemetryLog.txt");

        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            if (new FileInfo(filePath).Length == 0)
            {
                writer.WriteLine("Participant,Group,Asteroids Grabbed,Collisions (Total),Hand Accuracy (%),Average Completion Rate (Seconds),Total Play Time");
            }

            writer.WriteLine($"{participantID},{groupName},{asteroidsGrabbed},{totalCollisions},{handAccuracy:F1}%,{totalPlayTimeSeconds:F0}s,{formattedTime}");
        }

        Debug.Log($"Telemetry successfully saved to: {filePath}");
    }
}