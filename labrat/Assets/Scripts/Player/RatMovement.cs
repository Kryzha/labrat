using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RatMovement : MonoBehaviour
{
    [SerializeField]private float moveSpeed = 2f;
    [SerializeField]private float sprintSpeed = 4f;
    [SerializeField]private float mouseSensitivity = 3f;
    [SerializeField]private float jumpForce = 3f;

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

        verticalMove = Input.GetAxis("Vertical") * new Vector3(transform.forward.x, 0f, transform.forward.z);
        horizontalMouse = Input.GetAxis("Mouse X") * mouseSensitivity;

        Vector3 direction = verticalMove;
        currentRotation.y += horizontalMouse;

        // Плавне гальмування для усунення залишкового руху
        if (verticalMove == Vector3.zero && isGrounded)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
        else
        {
            transform.position += direction * currentSpeed * Time.deltaTime;
        }

        transform.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);
        cameraTransform.rotation = Quaternion.Euler(cameraTransform.rotation.x, currentRotation.y, 0f);
    }
    void Jump()
    {
        isGrounded = Physics.CheckSphere(transform.position, 0.1f, groundMask);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f)) // Замініть 1f на висоту, на якій ви хочете перевірити
        {
            if (hit.distance < 0.1f && Input.GetButtonDown("Jump"))
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }
}
