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
    public LayerMask GroundLayerMask;
    public float JumpHeight;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
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
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isGrounded)
            {
                Debug.Log("Jump");
                veclocity.y = Mathf.Sqrt(JumpHeight * -2 * gravity);
            }
        }
    }
}
