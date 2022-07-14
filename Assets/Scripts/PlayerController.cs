using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed;
    public Vector3 velocity;
    public float gravity = -20f;
    public float jumpVelocity;
    public bool isGrounded;
    public float maxJumpHoldTime = 3f;
    public float endOfJump = 0f;

    public Vector2 moveInput;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        bool jumpPressed = context.performed;

        if (jumpPressed && isGrounded)
        {
            // Start jump
            velocity.y = jumpVelocity;
            gravity = 0f;
            endOfJump = Time.time + maxJumpHoldTime;
        }

        bool jumpCanceled = context.canceled;
        if (jumpCanceled)
        {
            gravity = -20f;
        }
    }

    private void Update()
    {
        // Move
        Vector3 motion = moveInput.x * transform.right + moveInput.y * transform.forward;

        controller.Move(speed * Time.deltaTime * motion);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        // Ground
        const float radiusOffset = .01f;
        const float extraDist = .1f + radiusOffset;
        const float playerRadius = .5f - radiusOffset; // capsuleColider.radius
        const float distFromCenterToP2 = .5f; // center - height / 2 + radius
        const float maxSphereCastDist = distFromCenterToP2 + extraDist;

        Ray ray = new Ray(transform.position, Vector3.down);
        isGrounded = Physics.SphereCast(ray: ray, radius: playerRadius, maxDistance: maxSphereCastDist);
        Debug.Log($"IS GROUNDED {isGrounded}");

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Time.time > endOfJump)
        {
            gravity = -20f;
        }
    }
}
