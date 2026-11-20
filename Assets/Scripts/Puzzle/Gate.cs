using UnityEngine;

// Put on the gate/door object itself (needs a normal, non-trigger collider so it blocks).
// Open = slides by openOffset (default: sinks 3 units into the ground). Closed = back to start.
// Levers and pressure plates call SetOpen().
public class Gate : MonoBehaviour
{
    public Vector3 openOffset = new Vector3(0f, -3f, 0f);
    public float speed = 4f;               // units per second
    public bool startsOpen = false;
    public AudioSource sfx;                // optional: plays when the gate starts moving

    Vector3 closedPos;
    bool open;

    void Awake()
    {
        closedPos = transform.localPosition;
        open = startsOpen;
        if (open) transform.localPosition = closedPos + openOffset;
    }

    public void SetOpen(bool value)
    {
        if (open == value) return;         // no change, no sound
        open = value;
        if (sfx) sfx.Play();
    }

    void Update()
    {
        Vector3 target = open ? closedPos + openOffset : closedPos;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, speed * Time.deltaTime);
    }
}
