using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public LayerMask wall;

    [Header("Climbing")]
    [SerializeField]private float climbSpeed = 5f;
    [SerializeField]private float maxClimbTime;
    private float climbTimer;

    private bool climbing;

    [Header("Detection")]
    [SerializeField]private float detectionLength;
    [SerializeField]private float spreheCastRadius;
    [SerializeField]private float maxWallLookAngle;
    [SerializeField]private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    public RatMovement ratMovement;

    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, spreheCastRadius, orientation.forward, out frontWallHit, detectionLength, wall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        if(ratMovement.isGrounded)
        {
            climbTimer = maxClimbTime;
        }
    }

    private void StateMachine()
    {
        //State 1 - Climbing
        if(wallFront && Input.GetKey(KeyCode.Space) && wallLookAngle < maxWallLookAngle)
        {
            if(!climbing && climbTimer > 0)
            {
                StartClimbing();
            }

            if(climbTimer > 0)
            {
                climbTimer -= Time.deltaTime;
            }

            if(climbTimer < 0)
            {
                StopClimbing();
            }
        } else {
            if(climbing)
            {
                StopClimbing();
            }
        }
    }

    private void StartClimbing()
    {
        climbing = true;
    }

    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    } 
    private void StopClimbing()
    {
        climbing = false;
    }   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        WallCheck();
        StateMachine();

        if(climbing)
        {
            ClimbingMovement();
        }
    }
}
