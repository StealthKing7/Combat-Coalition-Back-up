using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovemet : MonoBehaviour
{

    private PlayerInput playerInput;
    private CharacterController characterController;
    public float Speed;
    public float MouseSensitivity;
    private MouseLook mouseLook;
    private Vector3 velocity;
    public float Gravity;
    public float JumpHeight;
    public Transform GroundCheck;
    public float GroundRadius;
    public LayerMask LayerMask;
    private bool isGrounded;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();  
        playerInput = new PlayerInput();
        playerInput.Player.Enable();
        mouseLook=GetComponentInChildren<MouseLook>();
        mouseLook.GetValues(MouseSensitivity, this.transform);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
       
    }

    void FixedUpdate()
    {
        Movement();
    }
    private void Movement()
    {
        float x, z;
        var delta = playerInput.Player.Movement.ReadValue<Vector2>();
        x = delta.x;
        z = delta.y;
        isGrounded = Physics.CheckSphere(GroundCheck.position, GroundRadius, LayerMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 move = transform.right * x + transform.forward * z;
        if (isGrounded)
        {
            characterController.Move(move * Speed * Time.deltaTime);
        }
        if (playerInput.Player.Jump.IsPressed() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }
        velocity.y += Gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

    }
}
