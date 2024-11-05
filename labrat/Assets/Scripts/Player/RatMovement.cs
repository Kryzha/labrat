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
    [SerializeField] private float climbSpeed = 2f;
    [SerializeField] private float wallDetectionDistance = 0.5f;  // Distance to detect walls ahead
    [SerializeField] private float climbUpwardForce = 1f;  // Upward force when starting to climb

    public bool isGrounded;
    public bool isJumping;
    private bool isClimbing;
    private bool isTouchingWall;

    private float horizontalMouse;
    private Rigidbody rb;
    private Quaternion currentRotation;
    private Vector3 verticalMove;
    public Transform cameraTransform;
    public LayerMask groundMask;
    public LayerMask wallMask;  // Mask for wall detection

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.drag = 2f;
        currentRotation = transform.rotation;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position, 0.05f, groundMask);

        // Handle jumping
        Jump();

        // Handle wall contact checking
        CheckWallContact();

        // Handle climbing input
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isClimbing)
            {
                StopClimbing();
            }
            else if (isTouchingWall)
            {
                StartClimbing();
            }
        }

        // Handle climbing movement
        if (isClimbing)
        {
            Climb();
        }
        else
        {
            // Handle regular movement
            Move();
        }
    }

    void Move()
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
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
            isGrounded = false;
        } if (isGrounded)
        {
            isJumping = false; // Rat has landed
        }
    }

    void CheckWallContact()
    {
        // Check if the rat is touching a wall
        isTouchingWall = Physics.CheckSphere(transform.position, 0.05f, wallMask);

        // Also check for walls slightly ahead of the rat to handle edges
        if (!isTouchingWall)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, wallDetectionDistance, wallMask))
            {
                isTouchingWall = true;
            }
        }
    }

    void StartClimbing()
    {
        isClimbing = true;
        isGrounded = false;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;  // Stop current movement
        rb.drag = 0f;  // Reduce drag while climbing

        // Move the rat slightly upward when starting to climb
        rb.AddForce(Vector3.up * climbUpwardForce, ForceMode.Impulse);

        // Adjust the rat's position upward to avoid being half in the ground
        transform.position += Vector3.up * 0.1f; // Adjust this value as needed

        // Orient rat on the wall
        transform.rotation = Quaternion.LookRotation(Vector3.up, -transform.forward);
    }

    void StopClimbing()
    {
        isClimbing = false;
        isGrounded = true;
        rb.useGravity = true;
        rb.drag = 2f;  // Restore drag
    }

    void Climb()
    {
        // Get the rat's forward and right direction
        Vector3 forward = transform.forward; // This will be the climbing up direction
        Vector3 right = transform.right;      // This will be the left/right movement direction

        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical");     // W/S

        // Calculate climb direction
        Vector3 climbDirection = (forward * vertical + right * horizontal) * climbSpeed;
        rb.velocity = climbDirection;

        // Set camera position while climbing
        cameraTransform.rotation = Quaternion.LookRotation(forward, Vector3.up);

        // Check if there's a wall ahead for smooth transition
        if (!isTouchingWall)
        {
            StopClimbing();
        }
        else
        {
            // Check for wall transitions
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, wallDetectionDistance, wallMask))
            {
                // Adjust the rat's position and orientation to the new wall
                transform.position = hit.point + hit.normal * 0.1f; // Adjust this value as needed
                transform.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
            }
        }
    }

}
