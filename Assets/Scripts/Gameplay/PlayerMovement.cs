using UnityEngine;
using UnityEngine.SceneManagement;

// Barebones WASD + jump using a CharacterController. Esc returns to the menu.
// Uses the old Input Manager: if Unity errors about "Input System package",
// set Project Settings > Player > Active Input Handling = Both.
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = -20f;
    public float jumpHeight = 1.5f;

    CharacterController cc;
    float verticalVelocity;

    void Awake() => cc = GetComponent<CharacterController>();

    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        if (cc.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;   // stick to ground
        if (cc.isGrounded && Input.GetButtonDown("Jump"))
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * speed;
        velocity.y = verticalVelocity;
        cc.Move(velocity * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SaveManager.Instance.Save();
            SceneManager.LoadScene(0);
        }
    }
}
