using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float sprintSpeed = 4f;
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float ledgeClimbUpForce = 2f;  // Force for climbing up the ledge
    [SerializeField] private float ledgeDetectionDistance = 0.5f;  // How far to detect a ledge
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask ledgeMask;  // Mask for detecting walls/ledges

    private bool isGrounded;
    private bool isGrabbingLedge;
    private bool isStuckToLedge = false; // Indicates if the rat is stuck to the ledge
    private Vector3 ledgePosition;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!isStuckToLedge)  // Only allow movement when not stuck to the ledge
        {
            Move();
            Jump();
        }
        CheckAndStickToLedge();

        // If a ledge is detected and player presses E, move to ledge and disable movement
        if (isGrabbingLedge && Input.GetKeyDown(KeyCode.E))
        {
            StickToLedge();
        }
    }

    // Basic movement for the rat
    private void Move()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        Vector3 moveDirection = transform.forward * Input.GetAxis("Vertical");

        Vector3 targetVelocity = moveDirection * currentSpeed;
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
    }

    // Jumping logic
    private void Jump()
    {
        isGrounded = Physics.CheckSphere(transform.position, 0.1f, groundMask);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // Check if the rat is near a ledge
    private void CheckAndStickToLedge()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = transform.forward;

        // Visualize the ray in the Scene view
        Debug.DrawRay(rayOrigin, rayDirection * ledgeDetectionDistance, Color.red, 0.1f);

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, ledgeDetectionDistance, ledgeMask))
        {
            // If a ledge is detected
            ledgePosition = hit.point; // Store the ledge position
            isGrabbingLedge = true;
            Debug.Log("Ledge detected! Press E to stick to it.");
        }
        else
        {
            isGrabbingLedge = false;
        }
    }

    // Method to stick the rat to the ledge and disable movement
    private void StickToLedge()
    {
        if (isGrabbingLedge)
        {
            // Stop movement and stick to the ledge
            rb.velocity = Vector3.zero;  // Stop the rat's current movement
            rb.useGravity = false;       // Disable gravity while on ledge
            transform.position = ledgePosition;  // Move the rat to the ledge position
            isStuckToLedge = true;       // Prevent further movement
            Debug.Log("Stuck to ledge. Camera movement enabled, player movement disabled.");
        }
    }

    // This method could be triggered to "unstick" the rat from the ledge
    public void UnstickFromLedge()
    {
        isStuckToLedge = false;
        rb.useGravity = true;  // Restore gravity
        Debug.Log("Unstuck from ledge. Movement enabled again.");
    }
}
