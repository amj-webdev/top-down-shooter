using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Camera mainCamera;
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;
    public float rotationSpeed = 700f;

    public float minZ = -5f;
    public float maxZ = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;  // Prevent rotation from physics forces
    }

    void Update()
    {
        if (Gamepad.current != null)
        {
            moveInput = Gamepad.current.leftStick.ReadValue();
            lookInput = Gamepad.current.rightStick.ReadValue();

            RotatePlayer();
        }
    }

    void FixedUpdate()
    {
        // Move using Rigidbody velocity instead of Transform.Translate()
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        rb.velocity = moveDirection * moveSpeed;

        // Keep camera position consistent with player position (only in X and Z)
        mainCamera.transform.position = new Vector3(transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);

        // Clamp the player's Z position within the allowed range
        Vector3 clampedPosition = transform.position;
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, minZ, maxZ);
        transform.position = clampedPosition;
    }

    private void RotatePlayer()
    {
        if (lookInput.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(-lookInput.y, lookInput.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
