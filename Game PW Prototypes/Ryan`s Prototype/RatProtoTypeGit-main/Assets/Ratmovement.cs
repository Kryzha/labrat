using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatMovement : MonoBehaviour
{
    private Rigidbody rb;
    private RigidbodyConstraints groundedConstraints;
    private Vector3 mousePos;

    [Header("Setup")]
    [Tooltip("How fast the rat runs")]
    public float moveSpeed = 20f;
    [Tooltip("How HIGH the rat jumps")]
    public float jumpPower = 600f;
    [Tooltip("How FAR the rat jumps")]
    public float jumpForce = 16f;

    [Tooltip("If true, can freely rotate while jumping")]
    public bool canSpin = false;

    public enum jumpFreedom
    {
        Locked,
        SteerAllowed,
        SpeedControl,
        FreeMovement
    }

    [Tooltip("Controls how much freedom player has while jumping")]
    public jumpFreedom jumpStyle = jumpFreedom.Locked;

    [Header("Debug")]
    public bool moveState = true;
    public bool isJump = false;

    private WallClimbing wallClimbing;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundedConstraints = rb.constraints;
        wallClimbing = GetComponent<WallClimbing>();
    }

    void Update()
    {
        if (wallClimbing.isClimbing)
        {
            return;
        }

        mousePos = Input.mousePosition;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        if (moveState || jumpStyle != jumpFreedom.Locked)
        {
            if (moveState || jumpStyle != jumpFreedom.SpeedControl)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, -angle, 0));
            }

            if (moveState || jumpStyle != jumpFreedom.SteerAllowed)
            {
                if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.W))
                {
                    rb.AddForce(transform.right * moveSpeed);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Space))
        {
            moveState = false;
            isJump = true;

            if (!canSpin)
                rb.constraints = rb.constraints | RigidbodyConstraints.FreezeRotationZ;

            rb.velocity = new Vector3(transform.right.x * jumpForce, jumpPower, transform.right.z * jumpForce);
        }

        if (isJump && jumpStyle == jumpFreedom.SteerAllowed && (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.W)))
        {
            rb.velocity = new Vector3(transform.right.x * jumpForce, rb.velocity.y, transform.right.z * jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            enterGrounded();
        }

        rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -moveSpeed, moveSpeed), rb.velocity.y, Mathf.Clamp(rb.velocity.z, -moveSpeed, moveSpeed));
    }

    void enterGrounded()
    {
        isJump = false;
        moveState = true;
        rb.constraints = groundedConstraints;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(!wallClimbing.isClimbing)
        {
            enterGrounded();
        }
    }
}