using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class PlayerMovement : MonoBehaviour
{
    PlayerInput inputActions;
    public event EventHandler OnMovement;
    public event EventHandler OnJump;
    public event EventHandler OnCameraMovement;
    [SerializeField] CharacterController characterController;
    [SerializeField] float Speed;
    [SerializeField] float JumpHieght;
    [SerializeField] float Gravity;
    [SerializeField] Transform GroundCheck;
    [SerializeField] float GroundCheckRaduis;
    [SerializeField] LayerMask GroundLayer;
    [SerializeField] float MouseSensitivity;
    Transform Maincam;
    bool IsGrounded;
    Vector3 move;
    Vector3 Velocity;
    private void Start()
    {
        inputActions = new PlayerInput();
        inputActions.PlayerMap.Enable();
        OnMovement += Movement;
        OnJump += Jump;
        Maincam = transform.GetChild(0);
       //nCameraMovement += CameraMovement;
    }


    private void Update()
    {
        Velocity.y += Gravity * Time.deltaTime;
        if (inputActions.PlayerMap.Movement.IsPressed())
        {
            OnMovement(this,EventArgs.Empty);
        }
        if (inputActions.PlayerMap.Jump.IsPressed())
        {
            OnJump(this,EventArgs.Empty);

        }
        /*if (Mouse.current.position.ReadValue()!=Vector2.zero)
        {
            OnCameraMovement(this, EventArgs.Empty);
        }*/
        CameraMovement();
        characterController.Move(Velocity * Time.deltaTime);
    }
    void Jump(object obj, EventArgs args)
    {
        Velocity.y = Mathf.Sqrt(JumpHieght * -2 * Gravity);
        IsGrounded = Physics.CheckSphere(GroundCheck.position, GroundCheckRaduis, GroundLayer);
        if(IsGrounded&&Velocity.y < 0)
        {
            Velocity.y = -2;
        }
    }
    private void Movement(object obj,EventArgs args)
    {
        move = transform.right*inputActions.PlayerMap.Movement.ReadValue<Vector2>().x+transform.forward* inputActions.PlayerMap.Movement.ReadValue<Vector2>().y;
        characterController.Move(move*Speed*Time.deltaTime);
    }
    void CameraMovement()
    {
        float Xrot = 0;
        Vector2 mouse = Mouse.current.delta.ReadValue();
        float x = mouse.x * MouseSensitivity * Time.deltaTime;
        float y = mouse.y * MouseSensitivity * Time.deltaTime;
        Xrot -= y;
        Xrot = Mathf.Clamp(Xrot, -90, 90);
        transform.Rotate(Vector3.up * x);
        Maincam.transform.localRotation = Quaternion.Euler(Xrot, 0, 0);

    }
}
