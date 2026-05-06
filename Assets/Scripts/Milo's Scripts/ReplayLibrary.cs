using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ReplayLibrary
{
    /// <summary>
    /// Returns a list of all ReplayData objects found in:
    /// - Resources/Replays/ (built‑in)
    /// - Application.persistentDataPath/Replays/ (recorded)
    /// </summary>
    public static List<ReplayData> GetAllReplays()
    {
        List<ReplayData> replays = new List<ReplayData>();

        // 1. Load built-in replays from Resources
        TextAsset[] builtInAssets = Resources.LoadAll<TextAsset>("Replays");
        foreach (var asset in builtInAssets)
        {
            ReplayData data = JsonUtility.FromJson<ReplayData>(asset.text);
            if (data != null && data.frames.Count > 0)
                replays.Add(data);
        }

        // 2. Load user-recorded replays from persistentDataPath
        string folder = Path.Combine(Application.persistentDataPath, "Replays");
        if (Directory.Exists(folder))
        {
            string[] files = Directory.GetFiles(folder, "*.rep");
            foreach (string file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    ReplayData data = JsonUtility.FromJson<ReplayData>(json);
                    if (data != null && data.frames.Count > 0)
                        replays.Add(data);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Failed to load replay {file}: {e.Message}");
                }
            }
        }

        return replays;
    }

    /// <summary>
    /// Returns a random replay from all available ones.
    /// </summary>
    public static ReplayData GetRandomReplay()
    {
        var all = GetAllReplays();
        if (all.Count == 0)
        {
            Debug.LogError("No replays found! Add at least one to Resources/Replays/");
            return null;
        }
        return all[Random.Range(0, all.Count)];
    }
}