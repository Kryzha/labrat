using System;
using System.Linq.Expressions;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class movement : MonoBehaviour
{
    [SerializeField]protected float speed, rotateSpeed, JumpForce, JumpReset;
    private float MoveX, MoveY, RotY,RotZ;
    public KeyCode JumpKey = KeyCode.Space;
    private float playerHeight;
    [SerializeField] private Rigidbody rb;
    public LayerMask GroundLayer;

    private WalkingState currentState;
    private WalkingState previousState;
    public enum WalkingState
    {
        grounded,
        climbing
    }

    private void Awake()
    {
        playerHeight = transform.localScale.y / 2;
    }
    void Update()
    {
        
        GetInput();
        RotRat();
        VelocityControl();
        if (currentState != previousState)
        {
            StateManager();
            previousState = currentState;
        }
    }


    private void FixedUpdate()
    {
        MoveRat();
    }
    void GetInput()
    {
        MoveX = Input.GetAxisRaw("Horizontal");
        MoveY = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(JumpKey) && Ground())
        {
            Jump();
            Debug.Log("jump");
        }
    }
    public void ChangeToClimbing()
    {
        ChangeState(WalkingState.climbing);
    }


    public void ChangeToWalking()
    {
        ChangeState(WalkingState.grounded);
    }  

    bool Ground()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, playerHeight + .1f, GroundLayer))
            return false;
        return true;
    }

    void Jump()
    {
        Vector3 JumpDir = transform.up;
        rb.AddForce(JumpDir * JumpForce * 10f, ForceMode.Force);
    }


    private void RotRat()
    {
        RotY += MoveX * rotateSpeed;
        transform.rotation = Quaternion.Euler(transform.rotation.x, RotY, RotZ);
    }

    void StateManager()
    {
        switch (currentState)
        {
            case WalkingState.grounded:
                RotZ = 0;
                break; 
            case WalkingState.climbing:
                RotZ =90f;
                break;
        }
    }
    

    bool ChangeState(WalkingState newState)
    {
        currentState = newState;
        Debug.Log($"new state: {currentState}");
        return true;
    }
    private void MoveRat()
    {
        Vector3 MoveDir = transform.right * MoveY;
        rb.AddForce(MoveDir.normalized * speed * 10f, ForceMode.Force);
    }

    private void VelocityControl() 
    {
        Vector3 FlatVel = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
        if(FlatVel.magnitude > speed)
        {
            Vector3 LimitVelocity = FlatVel.normalized * speed;
            rb.velocity = LimitVelocity;
        }
    }
}
