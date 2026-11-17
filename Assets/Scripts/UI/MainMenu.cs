using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Main menu: reads the save to set up the UI (the "load" side for settings + progress).
// Drag the UI objects into these slots in the Inspector.
public class MainMenu : MonoBehaviour
{
    public Button continueButton;
    public Button newGameButton;
    public Button[] levelButtons;      // element 0 = Level 1, element 1 = Level 2 ...
    public Slider volumeSlider;        // min 0, max 1
    public Toggle fullscreenToggle;

    void Start()
    {
        var save = SaveManager.Instance;
        var d = save.Data;

        // Settings: show saved values without firing the change events.
        if (volumeSlider)
        {
            volumeSlider.SetValueWithoutNotify(d.settings.masterVolume);
            volumeSlider.onValueChanged.AddListener(save.SetVolume);
        }
        if (fullscreenToggle)
        {
            fullscreenToggle.SetIsOnWithoutNotify(d.settings.fullscreen);
            fullscreenToggle.onValueChanged.AddListener(save.SetFullscreen);
        }

        // Progress: Continue only if there is something to continue.
        if (continueButton)
        {
            continueButton.interactable = d.progress.completedLevels.Count > 0 || d.progress.hasCheckpoint;
            continueButton.onClick.AddListener(() => LoadLevel(ContinueLevel()));
        }
        if (newGameButton)
            newGameButton.onClick.AddListener(NewGame);

        // Level select: lock levels the player hasn't unlocked yet.
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int level = i + 1;   // copy for the lambda
            levelButtons[i].interactable = level <= d.progress.highestLevelUnlocked;
            levelButtons[i].onClick.AddListener(() => LoadLevel(level));
        }
    }

    int ContinueLevel()
    {
        var p = SaveManager.Instance.Data.progress;
        int lastScene = SceneManager.sceneCountInBuildSettings - 1;
        // Never try to load a level that doesn't exist or isn't unlocked.
        return Mathf.Clamp(Mathf.Min(p.lastLevelPlayed, p.highestLevelUnlocked), 1, lastScene);
    }

    void NewGame()
    {
        SaveManager.Instance.ResetProgress();
        LoadLevel(1);
    }

    void LoadLevel(int level)
    {
        SaveManager.Instance.Save();   // also writes any settings changed in the menu
        SceneManager.LoadScene(level);
    }
}
