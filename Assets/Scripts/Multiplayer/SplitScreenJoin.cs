using UnityEngine;
using UnityEngine.InputSystem;

// Goes on the same object as the PlayerInputManager (Notification Behavior = Send Messages).
// PlayerInputManager spawns a Player prefab when someone presses a button on an unused device
// (keyboard -> player 1, gamepad -> player 2, or either order) and splits the screen itself.
// This script handles what it doesn't: where each player spawns, the extra audio listener,
// and hiding the "press to join" camera once someone joins.
public class SplitScreenJoin : MonoBehaviour
{
    public Transform[] spawnPoints;        // one empty object per player
    public GameObject lobbyCamera;         // overview camera (+ "Press any button to join" canvas as a child)

    // Called by PlayerInputManager when a player joins.
    void OnPlayerJoined(PlayerInput player)
    {
        int i = player.playerIndex;        // 0 = first to join, 1 = second

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Vector3 spawn = spawnPoints[i % spawnPoints.Length].position;
            player.GetComponent<PlayerMovement>().Teleport(spawn);
            var respawn = player.GetComponent<PlayerRespawn>();
            if (respawn) respawn.SetRespawn(spawn);
        }

        // Unity allows only one AudioListener; every Player prefab has one on its camera.
        if (i > 0)
        {
            var listener = player.GetComponentInChildren<AudioListener>();
            if (listener) listener.enabled = false;
        }

        if (lobbyCamera) lobbyCamera.SetActive(false);

        Debug.Log($"Player {i + 1} joined using {player.currentControlScheme}");
    }

    // Called by PlayerInputManager when a player leaves (e.g. controller unplugged).
    void OnPlayerLeft(PlayerInput player)
    {
        Debug.Log($"Player {player.playerIndex + 1} left");
    }
}
