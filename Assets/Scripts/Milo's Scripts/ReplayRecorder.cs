using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReplayRecorder : MonoBehaviour
{
    [Header("Recording Settings")]
    public float recordDuration = 120f;        // 2 minutes
    public float minSaveTime = 60f;            // don't save if less than 1 min
    public float sampleRate = 30f;             // frames per second

    private bool isRecording = false;
    private float recordStartTime;
    private float nextSampleTime;
    private ReplayData replayData;
    private bool hasAnyInput = false;

    void Update()
    {
        // Start recording on first movement key press
        if (!isRecording && !hasAnyInput)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
            {
                StartRecording();
            }
        }

        if (isRecording)
        {
            // Auto-stop after recordDuration
            if (Time.time - recordStartTime >= recordDuration)
            {
                StopRecordingAndSave();
                return;
            }

            // Sample at fixed rate
            if (Time.time >= nextSampleTime)
            {
                RecordFrame();
                nextSampleTime = Time.time + (1f / sampleRate);
            }
        }
    }

    void StartRecording()
    {
        isRecording = true;
        hasAnyInput = true;
        recordStartTime = Time.time;
        nextSampleTime = Time.time + (1f / sampleRate);
        replayData = new ReplayData();
        replayData.frameRate = sampleRate;
        replayData.startForward = transform.forward;
        replayData.frames.Clear();
        // Record initial frame immediately
        RecordFrame();
        Debug.Log("Recording started...");
    }

    void RecordFrame()
    {
        if (replayData != null)
        {
            replayData.frames.Add(new ReplayFrame(transform.position, transform.rotation));
        }
    }

    void StopRecordingAndSave()
    {
        isRecording = false;
        float recordedTime = Time.time - recordStartTime;
        if (recordedTime < minSaveTime)
        {
            Debug.Log($"Recording stopped early ({recordedTime:F1}s). File not saved.");
            replayData = null;
            return;
        }

        // Ensure folder exists
        string folder = Path.Combine(Application.persistentDataPath, "Replays");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        // Generate unique filename: Replay_YYYYMMDD_HHMMSS.rep
        string filename = $"Replay_{System.DateTime.Now:yyyyMMdd_HHmmss}.rep";
        string filePath = Path.Combine(folder, filename);

        string json = JsonUtility.ToJson(replayData, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"Replay saved to {filePath} (frames: {replayData.frames.Count})");
        replayData = null;
    }

    // If scene unloads before minSaveTime, discard (no save)
    void OnDestroy()
    {
        if (isRecording && (Time.time - recordStartTime) < minSaveTime)
        {
            Debug.Log("Recording destroyed early – no replay saved.");
        }
    }
}