using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RatMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float sprintSpeed = 4f;
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float jumpForce = 3f;

    public bool isGrounded;
    private float horizontalMouse;

    private Rigidbody rb;
    private Quaternion currentRotation;
    private Vector3 verticalMove;
    public Transform cameraTransform;
    public LayerMask groundMask;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;  // Prevent rotation from physics interactions
        rb.drag = 2f;              // Add drag to reduce sliding
        currentRotation = transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        verticalMove = Input.GetAxis("Vertical") * transform.forward;
        horizontalMouse = Input.GetAxis("Mouse X") * mouseSensitivity;

        // Set rotation based on mouse input
        currentRotation.y += horizontalMouse;
        transform.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);
        cameraTransform.rotation = Quaternion.Euler(cameraTransform.rotation.x, currentRotation.y, 0f);

        // Apply movement to Rigidbody velocity
        Vector3 targetVelocity = verticalMove * currentSpeed;
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
    }

    void Jump()
    {
        isGrounded = Physics.CheckSphere(transform.position, 0.1f, groundMask);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
