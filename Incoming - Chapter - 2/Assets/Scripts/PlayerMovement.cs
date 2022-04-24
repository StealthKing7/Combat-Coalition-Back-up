using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;
    [SerializeField]
    private float gravity;
    Vector3 veclocity;
    private bool isGrounded;
    public Transform GroundCheck;
    public float GroundCheckRadius;
    private PlayerInput inputActions;
    public LayerMask GroundLayerMask;
    public float JumpHeight;
    public float Speed;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        inputActions = new PlayerInput();
        inputActions.Enable();
        inputActions.Player.Jump.performed += Jump;
        inputActions.Player.Fire.performed += Shoot;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        isGrounded = Physics.CheckSphere(GroundCheck.position, GroundCheckRadius, GroundLayerMask);
        veclocity.y += gravity * Time.deltaTime;
        characterController.Move(veclocity * Time.deltaTime);
    }
    void FixedUpdate()
    {
        Vector2 input = inputActions.Player.Moment.ReadValue<Vector2>();
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        characterController.Move(move * Speed * Time.deltaTime);
    }
    void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Shot");
        }
    }
    void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isGrounded)
            {
                veclocity.y = Mathf.Sqrt(JumpHeight * -2 * gravity);
            }
        }
    }
}
