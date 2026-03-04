using UnityEngine;
using UnityEngine.InputSystem;

public class HorseMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] InputAction jump;
    [SerializeField] float jumpForce = 5f;
    Rigidbody rb;  
    private Vector3 moveDirection;
    private float rotationY;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        jump.Enable();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void FixedUpdate()
    {
        if (jump.WasPressedThisFrame())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        rotationY += mouseX * mouseSensitivity;

        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }
}