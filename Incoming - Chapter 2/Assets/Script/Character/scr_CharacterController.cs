using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static scr_Models;

public class scr_CharacterController : MonoBehaviour
{
    DefaultInputs DefaultInput;
    public Vector2 Input_Movement;
    public Vector2 Input_View;
    Vector3 NewCameraRotation;
    Vector3 NewCharacterRotation;
    CharacterController characterController;
    [Header("References")]
    [SerializeField] Transform CameraHolder;

    [Header("Player Settings")]
    [SerializeField] PlayerSettingModel PlayerSettings;
    [SerializeField] float ViewClampYmin = -70;
    [SerializeField] float ViewClampYmax = 80;
    [Header("Gravity")]
    [SerializeField] float GravityAmount;
    [SerializeField] float GravityMin;
    [SerializeField] float PlayerGravity;
    private void Awake()
    {
        DefaultInput = new DefaultInputs();
        DefaultInput.Character.Movement.performed += e => Input_Movement = e.ReadValue<Vector2>();
        DefaultInput.Character.View.performed += e => Input_View = e.ReadValue<Vector2>();
        DefaultInput.Character.Jump.performed += e => Jump();
        DefaultInput.Enable();

        NewCharacterRotation = transform.localRotation.eulerAngles;
        NewCameraRotation = CameraHolder.localRotation.eulerAngles;
        characterController = GetComponent<CharacterController>();
    }
    private void Update()
    {
        CalculateView();
        CalculateMovement();
    }
    void CalculateView()
    {
        NewCharacterRotation.y += PlayerSettings.ViewXSencitivity * (PlayerSettings.ViewXInverted ? -Input_View.x : Input_View.x) * Time.deltaTime;
        transform.localRotation=Quaternion.Euler(NewCharacterRotation);


        NewCameraRotation.x += PlayerSettings.ViewYSencitivity * (PlayerSettings.ViewYInverted? Input_View.y:-Input_View.y) * Time.deltaTime;
        NewCameraRotation.x = Mathf.Clamp(NewCameraRotation.x, ViewClampYmin, ViewClampYmax);
        CameraHolder.localRotation = Quaternion.Euler(NewCameraRotation);
    }
    void CalculateMovement()
    {
        var verticalSpeed = PlayerSettings.WalkingForwardSpeed * Input_Movement.y * Time.deltaTime;
        var horizontalspeed = PlayerSettings.WalkingStrafeSpeed * Input_Movement.x * Time.deltaTime;
        var newMovementSpeed = new Vector3(horizontalspeed, 0, verticalSpeed);
        newMovementSpeed=transform.TransformDirection(newMovementSpeed);
        if (PlayerGravity > GravityMin)
        {

            PlayerGravity -= GravityAmount * Time.deltaTime;
        }

        if (PlayerGravity < -1 && characterController.isGrounded)
        {
            PlayerGravity = -1;
        }
        newMovementSpeed.y += PlayerGravity;
        characterController.Move(newMovementSpeed);
    }
    void Jump()
    {
        if (!characterController.isGrounded)
            return;



    }
}
