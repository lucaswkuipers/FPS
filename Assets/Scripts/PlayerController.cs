using UnityEngine;
using UnityEngine.InputSystem;
using UniSense;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform gunTipTransform;
    [SerializeField] private float bulletVelocity;
    [SerializeField, Range(.1f, 1000f)] private float bulletsPerSecond;
    [SerializeField] float lowFreq;
    [SerializeField] float highFreq;
    [SerializeField] byte force;
    [SerializeField] Color color;
    public Vector3 velocity;
    public float gravity = -20f;
    public float jumpVelocity;
    public bool isGrounded;
    public float maxJumpHoldTime = 3f;
    public float endOfJump = 0f;
    public bool readyToShoot = true;
    public DualSenseGamepadHID dualsense;

    public Vector2 moveInput;
    public bool gunTriggerInput;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1);
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

    public void OnGunTrigger(InputAction.CallbackContext context)
    {
        gunTriggerInput = context.ReadValueAsButton();
    }

    private void FixedUpdate()
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

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Time.time > endOfJump)
        {
            gravity = -20f;
        }

        // Shoot
        if (gunTriggerInput && readyToShoot)
        {
            readyToShoot = false;
            GameObject bullet = Instantiate(bulletPrefab, position: gunTipTransform.position, rotation: Quaternion.identity);
            bullet.GetComponent<Rigidbody>().velocity = bulletVelocity * gunTipTransform.forward;
            Invoke(nameof(SetReadyToShootAsTrue), time: 1f / bulletsPerSecond);
        }

        if (dualsense == null)
        {
            dualsense = DualSenseGamepadHID.FindCurrent();
        }

        if (dualsense == null) return;

        dualsense.SetMotorSpeeds(lowFreq, highFreq);
        dualsense.SetLightBarColor(color);

        DualSenseGamepadState dualsenseState = new DualSenseGamepadState();

        DualSenseTriggerState triggerState = new DualSenseTriggerState();

        triggerState.Continuous.Force = force;

        dualsenseState.LeftTrigger = triggerState;
        dualsenseState.RightTrigger = triggerState;
        dualsenseState.Motor = new DualSenseMotorSpeed(lowFreq, highFreq);

        dualsense?.SetGamepadState(dualsenseState);
        dualsense.ResetHaptics();
    }

    private void SetReadyToShootAsTrue()
    {
        print("Can shoot!");
        readyToShoot = true;
    }
}