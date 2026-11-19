using UnityEngine;

// Goes on the frog MODEL (the child object that has the Animator), not the Player root.
// Reads how the parent CharacterController is actually moving, passes that to the
// Animator (Speed, Grounded), and turns the model to face the direction it moves.
// Rotating only the model keeps the camera (also a child of Player) from spinning.
[RequireComponent(typeof(Animator))]
public class FrogAnimatorDriver : MonoBehaviour
{
    public float turnSpeed = 720f;       // degrees per second
    public float groundedGrace = 0.1f;   // seconds off the ground before we count as "in the air"

    Animator anim;
    CharacterController cc;
    float airTime;

    // Hashing the parameter names once is faster than passing strings every frame.
    static readonly int SpeedHash = Animator.StringToHash("Speed");
    static readonly int GroundedHash = Animator.StringToHash("Grounded");

    void Awake()
    {
        anim = GetComponent<Animator>();
        cc = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        // cc.velocity = how fast the controller really moved last frame
        Vector3 v = cc.velocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);   // ignore up/down for run speed

        anim.SetFloat(SpeedHash, flat.magnitude);

        // isGrounded flickers for a frame on bumps and slopes; the grace timer
        // stops the jump animation from firing on every tiny step.
        airTime = cc.isGrounded ? 0f : airTime + Time.deltaTime;
        anim.SetBool(GroundedHash, airTime < groundedGrace);

        // Face the movement direction, turning smoothly.
        if (flat.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(flat);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
        }
    }
}
