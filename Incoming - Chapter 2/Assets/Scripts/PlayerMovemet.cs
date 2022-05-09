using UnityEngine;
public class PlayerMovemet : MonoBehaviour
{

    private PlayerInput playerInput;
    private CharacterController characterController;
    private float Speed;
    [SerializeField] private float MouseSensitivity;
    private MouseLook mouseLook;
    private Vector3 velocity;
    [SerializeField] private float Gravity;
    [SerializeField] private float JumpHeight;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private float GroundRadius;
    [SerializeField] private LayerMask LayerMask;
    private bool isGrounded;
    [SerializeField] private float SprintSpeed;
    [SerializeField] private float NormalSpeed;
    private Animator animator;
   [SerializeField] private AKM aKM;
    private bool isSprinting = false;
    private void Awake()
    {
        //aKM = GetComponentInChildren<AKM>();
        animator = GetComponentInChildren<Animator>();
        Speed = NormalSpeed;
        characterController = GetComponent<CharacterController>();
        playerInput = new PlayerInput();
        playerInput.Player.Enable();
        mouseLook = GetComponentInChildren<MouseLook>();
        mouseLook.GetValues(MouseSensitivity, transform, transform.GetChild(0));
    }
    private void Update()
    {
        if (playerInput.Player.Movement.IsPressed())
        {
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
        }
        if (playerInput.Player.Movement.IsPressed() && isSprinting)
        {
            animator.SetBool("Walk", false);
            if (playerInput.Player.Fire.IsPressed())
            {
                animator.SetBool("Run", false);
            }
            else
            {
                animator.SetBool("Run", true);
            }
        }
        else
        {
            animator.SetBool("Run", false);
        }
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


        if (playerInput.Player.Sprint.IsPressed())
        {
            Speed = SprintSpeed;
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
            Speed = NormalSpeed;
        }

        characterController.Move(move * Speed * Time.deltaTime);
        
        if (playerInput.Player.Jump.IsPressed() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }
        velocity.y += Gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

    }
}
