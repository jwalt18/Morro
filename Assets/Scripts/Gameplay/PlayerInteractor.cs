using UnityEngine;
using UnityEngine.InputSystem;

// Anything the player can "use" (levers now; NPCs, chests later) implements this.
public interface IInteractable
{
    void Interact(GameObject user);
}

// On the Player. When Interact is pressed (E on keyboard, Y/North on gamepad by default),
// finds the closest IInteractable within `radius` and uses it.
public class PlayerInteractor : MonoBehaviour
{
    public float radius = 1.5f;

    // Called by PlayerInput (Send Messages) for the Interact action.
    void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        IInteractable best = null;
        float bestDist = float.MaxValue;

        // Every collider within reach; the lever needs a collider for this to see it.
        foreach (Collider hit in Physics.OverlapSphere(transform.position, radius))
        {
            var target = hit.GetComponentInParent<IInteractable>();
            if (target == null) continue;

            float d = (hit.transform.position - transform.position).sqrMagnitude;
            if (d < bestDist) { bestDist = d; best = target; }
        }

        if (best != null) best.Interact(gameObject);
    }

    // Shows the reach as a yellow sphere when the Player is selected in the editor.
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
