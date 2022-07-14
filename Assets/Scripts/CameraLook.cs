using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    private float horizontalRotation = 0f;
    private Vector2 lookInput;

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 delta = Time.deltaTime * lookInput;
        horizontalRotation -= delta.y;
        horizontalRotation = Mathf.Clamp(horizontalRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(horizontalRotation, 0f, 0f);
        playerTransform.Rotate(Vector3.up * delta.x);
    }
}
