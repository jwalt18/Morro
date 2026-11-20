using System.Collections.Generic;
using UnityEngine;

// Gates stay open only while at least one player stands on the plate (prototype rule).
// Needs a collider with "Is Trigger" ticked, tall enough (~0.5) that the player's capsule enters it.
public class PressurePlate : MonoBehaviour
{
    public Gate[] gates;
    public Transform plateVisual;          // optional: the plate model, pushed down when pressed
    public float pressDepth = 0.05f;
    public AudioSource sfx;                // optional: plays when pressed

    // A set, not a counter: the same player can't be counted twice.
    readonly HashSet<Collider> standing = new HashSet<Collider>();
    Vector3 upPos;

    void Awake()
    {
        if (plateVisual) upPos = plateVisual.localPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (standing.Add(other) && standing.Count == 1) SetPressed(true);   // first one on
    }

    void OnTriggerExit(Collider other)
    {
        if (standing.Remove(other) && standing.Count == 0) SetPressed(false); // last one off
    }

    void SetPressed(bool pressed)
    {
        foreach (Gate g in gates)
            if (g) g.SetOpen(pressed);
        if (plateVisual)
            plateVisual.localPosition = upPos + (pressed ? Vector3.down * pressDepth : Vector3.zero);
        if (pressed && sfx) sfx.Play();
    }
}
