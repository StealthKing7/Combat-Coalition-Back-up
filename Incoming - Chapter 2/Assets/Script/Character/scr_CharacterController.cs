using UnityEngine;
using static scr_Models;

public class scr_CharacterController : MonoBehaviour
{
    DefaultInputs DefaultInput;
    [HideInInspector]
    public Vector2 Input_Movement;
    [HideInInspector]
    public Vector2 Input_View;
    Vector3 NewCameraRotation;
    Vector3 NewCharacterRotation;
    CharacterController characterController;
    [Header("References")]
    [SerializeField] Transform CameraHolder;
    [SerializeField] Transform feetTransfrom;

    [Header("Player Settings")]
    [SerializeField] PlayerSettingModel PlayerSettings;
    [SerializeField] float ViewClampYmin = -70;
    [SerializeField] float ViewClampYmax = 80;
    [SerializeField] LayerMask PlayerMask;

    [Header("Gravity")]
    [SerializeField] float GravityAmount;
    [SerializeField] float GravityMin;
    private  float PlayerGravity;
    [SerializeField] Vector3 JumpingForce;
    private Vector3 JumpingForceVelocity;

    [Header("Stance")]
    [SerializeField] PlayerStance playerStance;
    [SerializeField] float PlayerStanceSmoothing;
    [SerializeField] CharacterStance playerStandStance;
    [SerializeField] CharacterStance playerCrouchStance;
    [SerializeField] CharacterStance playerProneStance;
    private float StanceCheckErrorMargin = 0.05f;
    private float CameraHeight;
    private float CameraHeightVelocity;
    private Vector3 StanceCapsuleCenterVeloctiy;
    private float StanceCapsuleHieghtVelocity;
    private Vector3 newMovementSpeed;
    private Vector3 newMovementSpeedVelocity;
    private bool IsSprinting;
    [Header("Weapon")]
    public scr_WeaponController currentWeapon;
    private void Awake()
    {
        DefaultInput = new DefaultInputs();
        DefaultInput.Character.Movement.performed += e => Input_Movement = e.ReadValue<Vector2>();
        DefaultInput.Character.View.performed += e => Input_View = e.ReadValue<Vector2>();
        DefaultInput.Character.Jump.performed += e => Jump();
        DefaultInput.Character.Crouch.performed += e => Crouch();
        DefaultInput.Character.Prone.performed += e => Prone();
        DefaultInput.Character.Sprint.performed += e => ToggleSprint();
        DefaultInput.Character.SprintRealesed.performed += e => StopSprint();
        DefaultInput.Enable();
        NewCharacterRotation = transform.localRotation.eulerAngles;
        NewCameraRotation = CameraHolder.localRotation.eulerAngles;
        characterController = GetComponent<CharacterController>();
        CameraHeight = CameraHolder.localPosition.y;
        if (currentWeapon)
        {
            currentWeapon.Initialize(this);
            
        }
    }
    private void Update()
    {
        CalculateView();
        CalculateMovement();
        CalculateJump();
        CalculateStance();
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
        if (Input_Movement.y <= 0.2f)
        {
            IsSprinting = false;
        }
        var verticalSpeed = PlayerSettings.WalkingStrafeSpeed;
        var horizontalSpeed = PlayerSettings.WalkingForwardSpeed;
        if (IsSprinting)
        {
            verticalSpeed = PlayerSettings.RunningStrafeSpeed;
            horizontalSpeed = PlayerSettings.RunningForwardSpeed;
        }
        if (!characterController.isGrounded)
        {
            PlayerSettings.SpeedEffector = PlayerSettings.FallingSpeedEffector;
        }
        else if (playerStance == PlayerStance.Crouch)
        {
            PlayerSettings.SpeedEffector = PlayerSettings.CrouchSpeedEffector;
        } 
        else if (playerStance == PlayerStance.Prone)
        {
            PlayerSettings.SpeedEffector = PlayerSettings.ProneSpeedEffector;
        }
        else
        {
            PlayerSettings.SpeedEffector = 1;
        }


        verticalSpeed *= PlayerSettings.SpeedEffector;
        horizontalSpeed *= PlayerSettings.SpeedEffector;
        newMovementSpeed = Vector3.SmoothDamp(newMovementSpeed, new Vector3(verticalSpeed * Input_Movement.x * Time.deltaTime, 0, horizontalSpeed * Input_Movement.y * Time.deltaTime), ref newMovementSpeedVelocity, characterController.isGrounded ? PlayerSettings.MovementSmoothing : PlayerSettings.FallingSmoothing);
        var movementSpeed = transform.TransformDirection(newMovementSpeed);
        if (PlayerGravity > GravityMin)
        {

            PlayerGravity -= GravityAmount * Time.deltaTime;
        }

        if (PlayerGravity < -0.1f && characterController.isGrounded)
        {
            PlayerGravity = -0.1f;
        }

        movementSpeed.y += PlayerGravity;
        movementSpeed += JumpingForce * Time.deltaTime;
        characterController.Move(movementSpeed);
    }
    void CalculateStance()
    {
        var currentStance = playerStandStance;
        if (playerStance == PlayerStance.Crouch)
        {
            currentStance = playerCrouchStance;
        }else if(playerStance == PlayerStance.Prone)
        {
            currentStance = playerProneStance;
        }


        CameraHeight = Mathf.SmoothDamp(CameraHolder.localPosition.y, currentStance.CameraHeight, ref CameraHeightVelocity, PlayerStanceSmoothing);
        CameraHolder.localPosition = new Vector3(CameraHolder.localPosition.x, CameraHeight, CameraHolder.localPosition.z);

        characterController.height = Mathf.SmoothDamp(characterController.height, currentStance.StanceCollider.height, ref StanceCapsuleHieghtVelocity, PlayerStanceSmoothing);
        characterController.center = Vector3.SmoothDamp(characterController.center, currentStance.StanceCollider.center, ref StanceCapsuleCenterVeloctiy, PlayerStanceSmoothing);
    }
    void CalculateJump()
    {
        JumpingForce = Vector3.SmoothDamp(JumpingForce, Vector3.zero, ref JumpingForceVelocity, PlayerSettings.JumpingFallof);
    }
    void Jump()
    {
        if (!characterController.isGrounded || playerStance == PlayerStance.Prone)
        {
            return;
        }
        if (playerStance == PlayerStance.Crouch)
        {
            if (StanceCheck(playerStandStance.StanceCollider.height))
            {
                return;
            }

            playerStance = PlayerStance.Stand;
            return;
        }

        JumpingForce = Vector3.up * PlayerSettings.JumpingHeight;
        PlayerGravity = 0;
    }
    void Crouch()
    {
        if (playerStance == PlayerStance.Crouch)
        {
            if (StanceCheck(playerStandStance.StanceCollider.height))
            {
                return;
            }
            playerStance = PlayerStance.Stand;
            return;
        }
        if (StanceCheck(playerCrouchStance.StanceCollider.height))
        {
            return;
        }
        playerStance = PlayerStance.Crouch;
    }
    void Prone()
    {
        if (playerStance == PlayerStance.Prone)
        {
            if (StanceCheck(playerStandStance.StanceCollider.height))
            {
                return;
            }


            playerStance = PlayerStance.Stand;
            return;
        }
        playerStance = PlayerStance.Prone;
    }
     
    bool StanceCheck(float _StanceCheckHeight)
    {
        var Start = new Vector3(feetTransfrom.position.x, feetTransfrom.position.y + characterController.radius + StanceCheckErrorMargin, feetTransfrom.position.z);
        var End = new Vector3(feetTransfrom.position.x, feetTransfrom.position.y + _StanceCheckHeight - characterController.radius - StanceCheckErrorMargin, feetTransfrom.position.z);


        return Physics.CheckCapsule(Start, End, characterController.radius, PlayerMask);
    }

    void ToggleSprint()
    {
        if (Input_Movement.y <= 0.2f)
        {
            IsSprinting = false;
            return;
        }
        IsSprinting = !IsSprinting;
    }
    void StopSprint()
    {
        if(PlayerSettings.SprintHold)
        IsSprinting = false;
    }
}
