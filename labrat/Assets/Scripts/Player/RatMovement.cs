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
    public bool isJumping;
    public bool isClimbing;

    private float horizontalMouse;
    private Rigidbody rb;
    private Quaternion currentRotation;
    private Vector3 verticalMove;
    public Transform cameraTransform;
    public LayerMask groundMask;

    public WallClimbing wallClimbing;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.drag = 2f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Set collision detection mode
        currentRotation = transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        wallClimbing = GetComponent<WallClimbing>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position, 0.05f, groundMask);
        isClimbing = wallClimbing.isClimbing;

        // Handle climbing movement
        if (!isClimbing)
        {
            // Handle regular movement
            Move();
        }

        // Handle jumping
        Jump();

    }

    public void Move()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        verticalMove = Input.GetAxis("Vertical") * transform.forward;
        horizontalMouse = Input.GetAxis("Mouse X") * mouseSensitivity;

        currentRotation.y += horizontalMouse;
        transform.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);
        cameraTransform.rotation = Quaternion.Euler(cameraTransform.rotation.x, currentRotation.y, 0f);

        Vector3 targetVelocity = verticalMove * currentSpeed;
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
    }

    void Jump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            isGrounded = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        if (isGrounded)
        {
            isJumping = false; // Rat has landed
        }
    }

}
