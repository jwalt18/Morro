using System;
using System.IO;
using UnityEngine;

// One SaveManager exists for the whole game. It owns the SaveData in memory
// and is the only thing that reads/writes save.json on disk.
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public SaveData Data { get; private set; }

    const string FileName = "save.json";

    // persistentDataPath is a per-user folder Unity guarantees is writable.
    // Windows: C:\Users\<you>\AppData\LocalLow\<Company>\<Product>\save.json
    public string SavePath => Path.Combine(Application.persistentDataPath, FileName);

    // Runs automatically before the first scene loads, so the manager exists
    // even if you press Play from Level1 instead of the menu. No need to place it in a scene.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (Instance != null) return;
        new GameObject("SaveManager").AddComponent<SaveManager>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);   // survive scene changes
        Load();
        ApplySettings();
    }

    void OnApplicationQuit() => Save();

    // ---------- Disk I/O ----------

    public void Save()
    {
        Data.lastSaved = DateTime.Now.ToString("o");
        string json = JsonUtility.ToJson(Data, true);   // true = pretty-printed

        // Write to a temp file first, then swap it in. If the game crashes
        // mid-write, the old save.json is still intact.
        string tempPath = SavePath + ".tmp";
        try
        {
            File.WriteAllText(tempPath, json);
            if (File.Exists(SavePath)) File.Delete(SavePath);
            File.Move(tempPath, SavePath);
            Debug.Log($"Saved to {SavePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
        }
    }

    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            Data = new SaveData();               // first launch: defaults
            Debug.Log("No save file found, using defaults.");
            return;
        }
        try
        {
            string json = File.ReadAllText(SavePath);
            Data = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
            Debug.Log($"Loaded save from {SavePath}");
        }
        catch (Exception e)
        {
            // Corrupt or hand-edited badly: don't crash, start fresh.
            Debug.LogWarning($"Save file unreadable ({e.Message}), using defaults.");
            Data = new SaveData();
        }
    }

    // Copy the current save to / from any path (backup, sharing, testing).
    public void ExportTo(string path) => File.WriteAllText(path, JsonUtility.ToJson(Data, true));

    public bool ImportFrom(string path)
    {
        try
        {
            var imported = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
            if (imported == null) return false;
            Data = imported;
            ApplySettings();
            Save();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Import failed: {e.Message}");
            return false;
        }
    }

    // ---------- Settings ----------

    public void ApplySettings()
    {
        AudioListener.volume = Data.settings.masterVolume;
        Screen.fullScreen = Data.settings.fullscreen;
    }

    // Setters change memory + apply immediately. They do NOT write to disk on
    // every slider tick; the menu saves when you leave it, and quitting saves too.
    public void SetVolume(float v)
    {
        Data.settings.masterVolume = Mathf.Clamp01(v);
        ApplySettings();
    }

    public void SetFullscreen(bool on)
    {
        Data.settings.fullscreen = on;
        ApplySettings();
    }

    // ---------- Progress ----------

    public void MarkLevelStarted(int level)
    {
        Data.progress.lastLevelPlayed = level;
        Save();
    }

    public void SetCheckpoint(int level, Vector3 position)
    {
        var p = Data.progress;
        p.hasCheckpoint = true;
        p.checkpointLevel = level;
        p.checkpointPosition = position;
        Save();
    }

    public void CompleteLevel(int level)
    {
        var p = Data.progress;
        if (!p.completedLevels.Contains(level)) p.completedLevels.Add(level);
        p.highestLevelUnlocked = Mathf.Max(p.highestLevelUnlocked, level + 1);
        p.hasCheckpoint = false;          // checkpoint belonged to the finished level
        Save();
    }

    // New Game: wipe progress, keep settings.
    public void ResetProgress()
    {
        Data.progress = new ProgressData();
        Save();
    }
}
