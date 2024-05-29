using System;
using UnityEngine;
using TMPro;
using static scr_Models;
public class scr_CharacterController : MonoBehaviour
{
    #region - Parameters -
    //Private
    private CharacterController characterController;
    private Vector3 NewCameraRotation;
    private Vector3 NewCharacterRotation;
    private Vector3 StanceCapsuleCenterVeloctiy;
    private Vector3 newMovementSpeed;
    private Vector3 newMovementSpeedVelocity;
    private float StanceCheckErrorMargin = 0.05f;
    private float CameraHeight;
    private float CameraHeightVelocity;
    private float StanceCapsuleHieghtVelocity;
    private float CurrentLean;
    private float TargetLean;
    private float TargetLeanVelocity;
    private bool IsSprinting;
    //Events
    public event EventHandler<CharacterMovementAnimationEventArgs> CharacterMovementAnimationEvent;
    public class CharacterMovementAnimationEventArgs : EventArgs
    {
        public Vector3 dir;
        public bool isSprinting; 
    }
    public event EventHandler CharacterJumpAnimationEvent;
    public event EventHandler<OnStanceChangedEventArgs> OnStanceChanged;
    public class OnStanceChangedEventArgs : EventArgs
    {
        public PlayerStance stance;
    }


    [Header("References")]
    [SerializeField] Transform feetTransfrom;
    [SerializeField] GameObject[] CharacterModels;
    [field: SerializeField] public scr_InputManeger InputManeger { get; set; }
    [field: SerializeField] public Transform CameraHolder { get; private set; }
    [field: SerializeField] public Camera MainCamera { get; private set; }
    [Header("Player Settings")]
    [SerializeField] PlayerSettingModel PlayerSettings;
    [SerializeField] float ViewClampYmin = -60;
    [SerializeField] float ViewClampYmax = 60;
    [SerializeField] LayerMask PlayerMask;
    [SerializeField] LayerMask GroundMask;
    [Header("Gravity")]
    //[SerializeField] float GravityAmount;
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
    [field: SerializeField] public scr_WeaponController WeaponController { get; private set; }
    [Header("Weapon")]
    [SerializeField] float AnimationSpeed;
    [Header("Leaning")]
    [SerializeField] Transform LeanPiviot;
    [SerializeField] float LeanAngle;
    [SerializeField] float LeanSmoothing;
    #endregion

    #region - Awake/Start/Update -
    private void Awake()
    {
        scr_GameManeger.Instance.AddPlayer(this);
        Cursor.lockState = CursorLockMode.Locked;
        NewCharacterRotation = transform.localRotation.eulerAngles;
        NewCameraRotation = CameraHolder.localRotation.eulerAngles;
        characterController = GetComponent<CharacterController>();
        CameraHeight = CameraHolder.localPosition.y;
        WeaponController.SetCharcterController(this);
    }
    private void Start()
    {
        foreach(var child in CharacterModels)
        {
            child.layer = LayerMask.NameToLayer("Character");
        }
        InputManeger.Jump += Jump;
        InputManeger.Crouch += Crouch;
        InputManeger.Prone += Prone;
        InputManeger.ToggleSprint += ToggleSprint;
        InputManeger.StopSprint += StopSprint;
    }
    private void Update()
    {
        IsGrounded();
        IsFalling();
        CalculateView();
        CalculateMovement();
        CalculateJump();
        CalculateStance();
        CalculateLeaning();
    }
    #endregion

    #region - IsGrounded / IsFalling -
    bool IsGrounded()
    {
        return Physics.CheckSphere(feetTransfrom.position, PlayerSettings.IsGroundedRadius, GroundMask);
    }
    bool IsFalling()
    {
        return (!IsGrounded() && characterController.velocity.magnitude >= PlayerSettings.IsFallingSpeed);
    }
    #endregion


    #region -View/Movement-
    void CalculateView()
    {
        NewCharacterRotation.y += (WeaponController.GetWeapon().IsAiming ? PlayerSettings.AimSensitivityEffector : PlayerSettings.ViewXSencitivity) * (PlayerSettings.ViewXInverted ? -InputManeger.Input_View.x : InputManeger.Input_View.x) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(NewCharacterRotation);
        NewCameraRotation.x += (WeaponController.GetWeapon().IsAiming ? PlayerSettings.AimSensitivityEffector : PlayerSettings.ViewXSencitivity) * (PlayerSettings.ViewYInverted ? InputManeger.Input_View.y : -InputManeger.Input_View.y) * Time.deltaTime;
        NewCameraRotation.x = Mathf.Clamp(NewCameraRotation.x, ViewClampYmin, ViewClampYmax);
        CameraHolder.localRotation = Quaternion.Euler(NewCameraRotation + ((WeaponController.CurrentWeaponSO.WeaponType == WeaponType.Gun) ? ((WeaponController.GetWeapon() as scr_Gun).CamRecoil) : Vector3.zero));
    }
    void CalculateMovement()
    {
        if (InputManeger.Input_Movement.y <= 0.2f)
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
        if (!IsGrounded())
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
        else if (WeaponController.GetWeapon().IsAiming)
        {
            PlayerSettings.SpeedEffector = PlayerSettings.AimSpeedEffector;
        }
        else
        {
            PlayerSettings.SpeedEffector = 1;
        }

        AnimationSpeed = characterController.velocity.magnitude / (PlayerSettings.WalkingForwardSpeed * PlayerSettings.SpeedEffector);
        if (AnimationSpeed > 1)
        {
            AnimationSpeed = 1;
        }
        var direction = transform.InverseTransformDirection(characterController.velocity);
        CharacterMovementAnimationEvent?.Invoke(this, new CharacterMovementAnimationEventArgs
        {
            dir = direction.normalized,
            isSprinting = IsSprinting
        });
        verticalSpeed *= PlayerSettings.SpeedEffector;
        horizontalSpeed *= PlayerSettings.SpeedEffector;
        newMovementSpeed = Vector3.SmoothDamp(newMovementSpeed, new Vector3(verticalSpeed * InputManeger.Input_Movement.x * Time.deltaTime, 0, horizontalSpeed * InputManeger.Input_Movement.y * Time.deltaTime), ref newMovementSpeedVelocity, IsGrounded() ? PlayerSettings.MovementSmoothing : PlayerSettings.FallingSmoothing);
        var movementSpeed = transform.TransformDirection(newMovementSpeed);
        if (PlayerGravity < -0.1f && IsGrounded())
        {
            PlayerGravity = -0.1f;
        }
        if (PlayerGravity > GravityMin)
        {
            PlayerGravity += GravityVec.y * Time.deltaTime;
        }
        movementSpeed.y += PlayerGravity;
        movementSpeed += JumpingForce * Time.deltaTime;
        characterController.Move(movementSpeed);
    }
    #endregion


    #region - Leaning -
    void CalculateLeaning()
    {
        if (InputManeger.IsLeaningLeft)
        {
            TargetLean = LeanAngle;
        }
        else if (InputManeger.IsLeaningRight)
        {
            TargetLean = -LeanAngle;
        }
        else
        {
            TargetLean = 0;
        }

        CurrentLean = Mathf.SmoothDamp(CurrentLean, TargetLean, ref TargetLeanVelocity, LeanSmoothing);
        LeanPiviot.localRotation = Quaternion.Euler(new Vector3(0, 0, CurrentLean));
    } 

    #endregion


    #region - Jumping -
    void CalculateJump()
    {
        JumpingForce = Vector3.SmoothDamp(JumpingForce, Vector3.zero, ref JumpingForceVelocity, PlayerSettings.JumpingFallof);
    }
    void Jump()
    {
        if (!IsGrounded() || playerStance == PlayerStance.Prone)
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
        CharacterJumpAnimationEvent?.Invoke(this, EventArgs.Empty);
    }
    #endregion


    #region - Stance - 
    void CalculateStance()
    {
        var currentStance = playerStandStance;
        if (playerStance == PlayerStance.Crouch)
        {
            currentStance = playerCrouchStance;
        }
        else if (playerStance == PlayerStance.Prone)
        {
            currentStance = playerProneStance;
        }
        else if (playerStance == PlayerStance.Stand)
        {
            ViewClampYmin = -60;
        }
        if (IsFalling())
        {
            playerStance = PlayerStance.Stand;
        }

        CameraHeight = Mathf.SmoothDamp(CameraHolder.localPosition.y, currentStance.CameraHeight, ref CameraHeightVelocity, PlayerStanceSmoothing);
        CameraHolder.localPosition = new Vector3(CameraHolder.localPosition.x, CameraHeight, currentStance.ForwardPos);
        characterController.height = Mathf.SmoothDamp(characterController.height, currentStance.StanceCollider.height, ref StanceCapsuleHieghtVelocity, PlayerStanceSmoothing);
        characterController.center = Vector3.SmoothDamp(characterController.center, currentStance.StanceCollider.center, ref StanceCapsuleCenterVeloctiy, PlayerStanceSmoothing);
        OnStanceChanged?.Invoke(this, new OnStanceChangedEventArgs { stance = playerStance });
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
        ViewClampYmin = -60;
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
        ViewClampYmin = 0;
    }
    bool StanceCheck(float _StanceCheckHeight)
    {
        var Start = new Vector3(feetTransfrom.position.x, feetTransfrom.position.y + characterController.radius + StanceCheckErrorMargin, feetTransfrom.position.z);
        var End = new Vector3(feetTransfrom.position.x, feetTransfrom.position.y + _StanceCheckHeight - characterController.radius - StanceCheckErrorMargin, feetTransfrom.position.z);
        return Physics.CheckCapsule(Start, End, characterController.radius, PlayerMask);
    }
    #endregion


    #region - Sprinting -
    void ToggleSprint()
    {
        if (InputManeger.Input_Movement.y <= 0.2f)
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
    #endregion

    #region - Gizmos -
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(feetTransfrom.position, PlayerSettings.IsGroundedRadius);
    }
    #endregion

}
