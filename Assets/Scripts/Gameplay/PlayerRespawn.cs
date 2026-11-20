using UnityEngine;

// Each player remembers their own respawn point. Checkpoints update it,
// KillZones (water, pits, traps) send the player back to it.
[RequireComponent(typeof(PlayerMovement))]
public class PlayerRespawn : MonoBehaviour
{
    Vector3 respawnPoint;
    PlayerMovement movement;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        respawnPoint = transform.position;     // default: wherever the player started
    }

    public void SetRespawn(Vector3 point) => respawnPoint = point;

    public void Respawn() => movement.Teleport(respawnPoint);
}
