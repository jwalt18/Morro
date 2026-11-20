using UnityEngine;

// Water, pits, traps. Collider with "Is Trigger" ticked.
// Any player who touches it goes back to their last checkpoint.
public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var respawn = other.GetComponent<PlayerRespawn>();
        if (respawn) respawn.Respawn();
    }
}
