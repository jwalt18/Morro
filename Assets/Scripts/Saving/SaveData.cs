using System;
using System.Collections.Generic;
using UnityEngine;

// Plain data classes. No MonoBehaviour, no logic: just the shape of the save file.
// [Serializable] + public fields = what JsonUtility can read and write.
// JsonUtility does NOT handle: properties, Dictionaries, or top-level arrays.

[Serializable]
public class SettingsData
{
    public float masterVolume = 1f;   // 0..1, applied to AudioListener.volume
    public bool fullscreen = true;    // applied to Screen.fullScreen
}

[Serializable]
public class ProgressData
{
    // Level numbers = scene build index (0 = MainMenu, 1 = Level1, 2 = Level2 ...)
    public int highestLevelUnlocked = 1;
    public int lastLevelPlayed = 1;
    public List<int> completedLevels = new List<int>();

    // Player state: where to respawn when continuing mid-level
    public bool hasCheckpoint = false;
    public int checkpointLevel = 0;
    public Vector3 checkpointPosition = Vector3.zero;
}

[Serializable]
public class SaveData
{
    public int version = 1;           // bump if you change the format later
    public string lastSaved = "";
    public SettingsData settings = new SettingsData();
    public ProgressData progress = new ProgressData();
}
