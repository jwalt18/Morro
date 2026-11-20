using UnityEngine;
using UnityEngine.InputSystem;

// REPLACES the old PlayerMovement. Same move/jump/gravity math, but input now comes
// from a PlayerInput component (Behavior = Send Messages), so each player reads only
// their own device: one on keyboard, one on a gamepad.
//
// PlayerInput calls OnMove / OnJump automatically, because the methods are named
// "On" + the action name in the Input Actions asset (Move, Jump).
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -20f;
    public float jumpHeight = 3f;

    CharacterController cc;
    float verticalVelocity;
    Vector2 moveInput;
    bool jumpQueued;

    void Awake() => cc = GetComponent<CharacterController>();

    // Called by PlayerInput when the Move action changes (and with zero when released).
    void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    // Called by PlayerInput when Jump is pressed.
    void OnJump(InputValue value)
    {
        if (value.isPressed) jumpQueued = true;
    }

    void Update()
    {
        Vector3 move = Vector3.ClampMagnitude(new Vector3(moveInput.x, 0f, moveInput.y), 1f);

        if (cc.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;   // stick to ground
        if (jumpQueued && cc.isGrounded)
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        jumpQueued = false;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * speed;
        velocity.y = verticalVelocity;
        cc.Move(velocity * Time.deltaTime);
    }

    // Safe way to move a CharacterController instantly (spawns, respawns).
    public void Teleport(Vector3 position)
    {
        // May be called before Awake (PlayerInputManager announces the join from
        // PlayerInput's OnEnable, which can run before this script's Awake).
        if (cc == null) cc = GetComponent<CharacterController>();
        cc.enabled = false;            // CharacterController overrides transform.position while enabled
        transform.position = position;
        cc.enabled = true;
        verticalVelocity = 0f;
    }
}
