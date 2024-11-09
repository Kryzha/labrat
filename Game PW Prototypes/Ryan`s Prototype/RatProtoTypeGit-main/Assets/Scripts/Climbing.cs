using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
   [SerializeField] private float climbSpeed = 2f;
    [SerializeField] private float wallDetectionDistance = 0.5f;
    [SerializeField] private float climbUpwardForce = 1f;

    private bool isClimbing;
    private bool isTouchingWall;
    private Rigidbody rb;
    private Transform cameraTransform;
    public LayerMask wallMask;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
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
            }
        }
    }

    void StartClimbing()
    {
        isClimbing = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.drag = 0f;
        rb.AddForce(Vector3.up * climbUpwardForce, ForceMode.Impulse);
        transform.position += Vector3.up * 0.1f;
        transform.rotation = Quaternion.LookRotation(Vector3.up, -transform.forward);
    }

    void StopClimbing()
    {
        isClimbing = false;
        rb.useGravity = true;
        rb.drag = 2f;
    }

    void Climb()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 climbDirection = (forward * vertical + right * horizontal) * climbSpeed;
        rb.velocity = climbDirection;

        cameraTransform.rotation = Quaternion.LookRotation(forward, Vector3.up);

        if (!isTouchingWall)
        {
            StopClimbing();
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, wallDetectionDistance, wallMask))
            {
                transform.position = hit.point + hit.normal * 0.1f;
                transform.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
            }
        }
    }
}
