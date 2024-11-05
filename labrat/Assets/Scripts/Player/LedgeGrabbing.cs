using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class LedgeGrabbing : MonoBehaviour
{
    [SerializeField] private float ledgeClimbUpForce = 2f;   // Force for climbing up the ledge
    [SerializeField] private LayerMask ledgeMask;  // Mask for detecting walls/ledges
    [SerializeField] private float ledgeDetectionDistance = 0.5f;  // How far to detect a ledge

    private bool isGrabbingLedge;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CheckForLedge();

        if (isGrabbingLedge && Input.GetKeyDown(KeyCode.E))
        {
            ClimbLedge();
        }
    }

    private void CheckForLedge()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, ledgeDetectionDistance, ledgeMask))
        {
            // If a ledge is detected
            if (hit.collider != null)
            {
                isGrabbingLedge = true;
            }
        }
        else
        {
            isGrabbingLedge = false;
        }
    }

    private void ClimbLedge()
    {
        if (isGrabbingLedge)
        {
            // Disable gravity and add upward force to climb
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * ledgeClimbUpForce, ForceMode.Impulse);

            // Once you've climbed, restore gravity
            Invoke(nameof(EndClimb), 0.5f);  // Delay to allow the character to finish climbing
        }
    }

    private void EndClimb()
    {
        rb.useGravity = true;
        isGrabbingLedge = false;
    }
}
