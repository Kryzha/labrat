using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClimbing : MonoBehaviour
{
    [SerializeField] private float climbSpeed = 2f;
    [SerializeField] private float wallDetectionDistance = 0.01f;
    [SerializeField] private float climbUpwardForce = 1f;

    public bool isTouchingWall;
    public bool isClimbing;
    public bool isGrounded;

    private Rigidbody rb;
    public LayerMask wallMask;
    public Transform cameraTransform;
    public RatMovement ratMovement;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraTransform = GetComponent<RatMovement>().cameraTransform;
        ratMovement = GetComponent<RatMovement>();
    }

    void Update()
    {
        isGrounded = ratMovement.isGrounded;

        CheckWallContact();

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

        if (isClimbing)
        {
            Climb();
        }
    }

    void CheckWallContact()
    {
        isTouchingWall = Physics.CheckSphere(transform.position, 0.05f, wallMask);

        if (!isTouchingWall)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, wallDetectionDistance, wallMask))
            {
                isTouchingWall = true;
                Debug.Log("Wall detected ahead: " + hit.collider.name);
            }
            else
            {
                Debug.LogWarning("No wall detected");
            }
        }
    }

    void StartClimbing()
    {
        Debug.Log("StartClimbing called");
        isClimbing = true;
        isGrounded = false;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.drag = 0f;

        rb.AddForce(Vector3.up * climbUpwardForce, ForceMode.Impulse);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, wallDetectionDistance, wallMask))
        {
            Vector3 wallNormal = hit.normal;
            Quaternion targetRotation = Quaternion.LookRotation(-wallNormal, Vector3.up) * Quaternion.Euler(90f, 0f, 0f);
            
            Debug.Log($"Current Rotation: {transform.rotation.eulerAngles}, Target Rotation: {targetRotation.eulerAngles}");
            transform.rotation = targetRotation;
        }
    }

    void StopClimbing()
    {
        Debug.Log("StopClimbing called");
        isClimbing = false;
        isGrounded = true;
        rb.useGravity = true;
        rb.drag = 2f;
    }

    public void Climb()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 climbDirection = (forward * vertical + right * horizontal) * climbSpeed;
        rb.velocity = climbDirection;

        if (!isTouchingWall)
        {
            StopClimbing();
        }
    }
}
