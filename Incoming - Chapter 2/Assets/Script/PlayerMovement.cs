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
    [SerializeField] float SprintSpeed;
    [SerializeField] float JumpHieght;
    [SerializeField] float Gravity;
    [SerializeField] Transform GroundCheck;
    [SerializeField] float GroundCheckRaduis;
    [SerializeField] LayerMask GroundLayer;
    [SerializeField] float MouseSensitivity;
    Transform Maincam;
    bool IsGrounded = true;
    Vector3 move;
    Vector3 Velocity;
    float Xrot = 0;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        inputActions = new PlayerInput();
        inputActions.PlayerMap.Enable();
        OnMovement += Movement;
        OnJump += Jump;
        Maincam = transform.GetChild(0);
        OnCameraMovement += CameraMovement;
    }


    private void Update()
    {
        Velocity.y += Gravity * Time.deltaTime;
        if (inputActions.PlayerMap.Movement.IsPressed()&&IsGrounded)
        {
            OnMovement?.Invoke(this,EventArgs.Empty);
        }
        if (inputActions.PlayerMap.Jump.IsPressed()&&IsGrounded)
        {
            OnJump?.Invoke(this,EventArgs.Empty);

        }
        if (Mouse.current.position.ReadValue()!=Vector2.zero)
        {
            OnCameraMovement?.Invoke(this, EventArgs.Empty);
        }

        IsGrounded = Physics.CheckSphere(GroundCheck.position, GroundCheckRaduis, GroundLayer);
        characterController.Move(Velocity * Time.deltaTime);
    }
    void Jump(object obj, EventArgs args)
    {
        Velocity.y = Mathf.Sqrt(JumpHieght * -2 * Gravity);
        if(IsGrounded&&Velocity.y < 0)
        {
            Velocity.y = -2;
        }
    }
    private void Movement(object obj,EventArgs args)
    {
        float speed = Speed;
        move = transform.right*inputActions.PlayerMap.Movement.ReadValue<Vector2>().x+transform.forward* inputActions.PlayerMap.Movement.ReadValue<Vector2>().y;
        if (inputActions.PlayerMap.Sprint.IsPressed())
        {
            speed = SprintSpeed;
        }
        characterController.Move(move*speed*Time.deltaTime);
    }
    void CameraMovement(object obj, EventArgs args)
    {

        float x = inputActions.PlayerMap.MouseX.ReadValue<float>() * MouseSensitivity/2 * Time.deltaTime;
        float y = inputActions.PlayerMap.MouseY.ReadValue<float>() * MouseSensitivity * Time.deltaTime;
        Xrot -= y;
        Xrot = Mathf.Clamp(Xrot, -90, 90);
        Maincam.transform.localRotation = Quaternion.Euler(Xrot, 0, 0);
        transform.Rotate(Vector3.up * x);

    }
}
