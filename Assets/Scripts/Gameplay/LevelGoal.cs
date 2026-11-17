using UnityEngine;
using UnityEngine.SceneManagement;

// Put on an object with a Collider set to "Is Trigger" at the end of a level.
// Marks the level complete, unlocks the next one, saves, then loads the next scene.
public class LevelGoal : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        int level = SceneManager.GetActiveScene().buildIndex;
        SaveManager.Instance.CompleteLevel(level);

        int next = level + 1;
        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            SceneManager.LoadScene(0);   // last level done: back to menu
    }
}
