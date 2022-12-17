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
    private  float PlayerGravity;
    [SerializeField] Vector3 JumpingForce;
    private Vector3 JumpingForceVelocity;
    [Header("Stance")]
    [SerializeField] PlayerStance playerStance;
    [SerializeField] float PlayerStanceSmoothing;
    [SerializeField] float CameraStandHeight,CameraCrouchHeight ,CameraProneHeight;
    private float CameraHeight;
    private float CameraHeightVelocity;
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
        CameraHeight = CameraHolder.localPosition.y;
    }
    private void Update()
    {
        CalculateView();
        CalculateMovement();
        CalculateJump();
        CalculateCameraHeight();
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

        if (PlayerGravity < -0.1f && characterController.isGrounded)
        {
            PlayerGravity = -0.1f;
        }

        newMovementSpeed.y += PlayerGravity;
        newMovementSpeed += JumpingForce * Time.deltaTime;
        characterController.Move(newMovementSpeed);
    }
    void CalculateCameraHeight()
    {
        var StanceHeight = CameraStandHeight;
        if (playerStance == PlayerStance.Crouch)
        {
            StanceHeight = CameraCrouchHeight;
        }else if(playerStance == PlayerStance.Prone)
        {
            StanceHeight = CameraProneHeight;
        }


        CameraHeight = Mathf.SmoothDamp(CameraHolder.localPosition.y, StanceHeight, ref CameraHeightVelocity, PlayerStanceSmoothing);
        CameraHolder.localPosition = new Vector3(CameraHolder.localPosition.x, CameraHeight, CameraHolder.localPosition.z);
    }
    void CalculateJump()
    {
        JumpingForce = Vector3.SmoothDamp(JumpingForce, Vector3.zero, ref JumpingForceVelocity, PlayerSettings.JumpingFallof);
    }
    void Jump()
    {
        if (!characterController.isGrounded)
            return;

        JumpingForce = Vector3.up * PlayerSettings.JumpingHeight;
        PlayerGravity = 0;
    }
}
