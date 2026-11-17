using UnityEngine;
using UnityEngine.SceneManagement;

// Put on an object with a Collider set to "Is Trigger".
// Player walks through it -> position saved.
public class Checkpoint : MonoBehaviour
{
    public Vector3 respawnOffset = new Vector3(0f, 1f, 0f);   // spawn slightly above the marker
    bool used;

    void OnTriggerEnter(Collider other)
    {
        if (used || !other.CompareTag("Player")) return;
        used = true;
        int level = SceneManager.GetActiveScene().buildIndex;
        SaveManager.Instance.SetCheckpoint(level, transform.position + respawnOffset);
        Debug.Log($"Checkpoint reached in level {level}");
    }
}
