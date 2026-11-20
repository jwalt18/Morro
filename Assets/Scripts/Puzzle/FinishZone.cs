using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// The finish tile. Collider with "Is Trigger" ticked.
// Level ends only when EVERY joined player is standing in it (prototype rule).
// With one player joined (solo testing) that one player is enough.
// Replaces LevelGoal for co-op levels; same save + load-next-level behavior.
public class FinishZone : MonoBehaviour
{
    readonly HashSet<PlayerInput> inside = new HashSet<PlayerInput>();
    bool done;

    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerInput>();
        if (player == null) return;
        inside.Add(player);
        CheckFinished();
    }

    void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<PlayerInput>();
        if (player != null) inside.Remove(player);
    }

    void CheckFinished()
    {
        if (done) return;
        int needed = Mathf.Max(1, PlayerInput.all.Count);   // all players currently joined
        if (inside.Count < needed) return;

        done = true;
        int level = SceneManager.GetActiveScene().buildIndex;
        SaveManager.Instance.CompleteLevel(level);

        int next = level + 1;
        SceneManager.LoadScene(next < SceneManager.sceneCountInBuildSettings ? next : 0);
    }
}
