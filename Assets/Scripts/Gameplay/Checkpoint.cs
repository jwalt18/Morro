using UnityEngine;
using UnityEngine.SceneManagement;

// REPLACES the old Checkpoint. Collider with "Is Trigger" ticked.
// Now sets the respawn point for EVERY player who passes (co-op), and still writes the
// checkpoint to save.json once, like before.
public class Checkpoint : MonoBehaviour
{
    public Vector3 respawnOffset = new Vector3(0f, 1f, 0f);   // spawn slightly above the marker
    bool saved;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Vector3 point = transform.position + respawnOffset;

        var respawn = other.GetComponent<PlayerRespawn>();
        if (respawn) respawn.SetRespawn(point);

        if (saved) return;
        saved = true;
        SaveManager.Instance.SetCheckpoint(SceneManager.GetActiveScene().buildIndex, point);
    }
}
