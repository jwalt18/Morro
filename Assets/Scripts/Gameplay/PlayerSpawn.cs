using UnityEngine;
using UnityEngine.SceneManagement;

// On level start: record which level we're in, and if the save has a
// checkpoint for THIS level, move the player there (the "load" side).
public class PlayerSpawn : MonoBehaviour
{
    void Start()
    {
        int level = SceneManager.GetActiveScene().buildIndex;
        var save = SaveManager.Instance;
        save.MarkLevelStarted(level);

        var p = save.Data.progress;
        if (p.hasCheckpoint && p.checkpointLevel == level)
        {
            // A CharacterController overrides transform.position,
            // so it must be disabled while we teleport.
            var cc = GetComponent<CharacterController>();
            if (cc) cc.enabled = false;
            transform.position = p.checkpointPosition;
            if (cc) cc.enabled = true;
        }
    }
}
