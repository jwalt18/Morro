using UnityEngine;

// A lever the player interacts with. Each use flips it and opens/closes its gates.
// Needs a collider (trigger or not) so PlayerInteractor can find it.
public class Lever : MonoBehaviour, IInteractable
{
    public Gate[] gates;                   // drag the gate(s) this lever controls
    public Transform handle;               // optional: the part that tilts
    public float onAngle = -45f;
    public float offAngle = 45f;
    public AudioSource sfx;                // optional click sound

    bool on;

    void Start() => ApplyHandle();

    public void Interact(GameObject user)
    {
        on = !on;
        foreach (Gate g in gates)
            if (g) g.SetOpen(on);
        ApplyHandle();
        if (sfx) sfx.Play();
    }

    void ApplyHandle()
    {
        if (handle) handle.localRotation = Quaternion.Euler(on ? onAngle : offAngle, 0f, 0f);
    }
}
