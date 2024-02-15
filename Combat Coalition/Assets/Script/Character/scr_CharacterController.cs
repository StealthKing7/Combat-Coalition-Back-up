using System;
using UnityEngine;
using UnityEngine.UI;
using static scr_Models;
public class scr_CharacterController : MonoBehaviour
{
    #region - Parameters -
    //Private
    private scr_InputManeger inputManeger;
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
    private float frameRate;
    private float FpsTimer;
    private float DefautYPos;
    private float HeadBobTimer;
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
    public event EventHandler<OnFpsUpdateTextEventArgs> OnFpsUpdateText;
    public class OnFpsUpdateTextEventArgs : EventArgs
    {
        public float FrameRate;
    }

    [Header("References")]
    [SerializeField] Transform CameraTarget;
    [field: SerializeField] public Transform CameraHolder { get; private set; }
    [field: SerializeField] public Camera MainCamera { get; private set; }
    [field: SerializeField] public Text FPSText { get; private set; }
    [SerializeField] Transform feetTransfrom;
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

    #region - Awake/Start/Update/LateUpdate -
    private void Awake()
    {
        scr_GameManeger.Instance.AddPlayer(this);
        Cursor.lockState = CursorLockMode.Locked;
        NewCharacterRotation = transform.localRotation.eulerAngles;
        NewCameraRotation = CameraHolder.localRotation.eulerAngles;
        characterController = GetComponent<CharacterController>();
        CameraHeight = CameraHolder.localPosition.y;
        WeaponController.SetCharcterController(this);
        DefautYPos = CameraTarget.localPosition.y;
    }
    private void Start()
    {
        inputManeger = scr_InputManeger.Instance;
        inputManeger.Jump += Jump;
        inputManeger.Crouch += Crouch;
        inputManeger.Prone += Prone;
        inputManeger.ToggleSprint += ToggleSprint;
        inputManeger.StopSprint += StopSprint;
    }
    private void Update()
    {
        OnFpsUpdateText?.Invoke(this,new OnFpsUpdateTextEventArgs { FrameRate = frameRate });
        if (FpsTimer > 1f)
        {
            frameRate = (int)(1f / Time.unscaledDeltaTime);
            FpsTimer = 0f;
        }
        else
        {
            FpsTimer += Time.deltaTime;
        }
        IsGrounded();
        IsFalling();
        CalculateView();
        CalculateMovement();
        CalculateHeadBob();
        CalculateJump();
        CalculateStance();
        CalculateLeaning();
    }
    private void LateUpdate()
    {
        CalculateCameraPosition();
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
        NewCharacterRotation.y += (WeaponController.GetWeapon().IsAiming ? PlayerSettings.AimSensitivityEffector : PlayerSettings.ViewXSencitivity) * (PlayerSettings.ViewXInverted ? -inputManeger.Input_View.x : inputManeger.Input_View.x) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(NewCharacterRotation);
        NewCameraRotation.x += (WeaponController.GetWeapon().IsAiming ? PlayerSettings.AimSensitivityEffector : PlayerSettings.ViewXSencitivity) * (PlayerSettings.ViewYInverted ? inputManeger.Input_View.y : -inputManeger.Input_View.y) * Time.deltaTime;
        NewCameraRotation.x = Mathf.Clamp(NewCameraRotation.x, ViewClampYmin, ViewClampYmax);
        CameraHolder.localRotation = Quaternion.Euler(NewCameraRotation + ((WeaponController.weaponSO.WeaponType == WeaponType.Gun) ? ((WeaponController.GetWeapon() as scr_Gun).CamRecoil) : Vector3.zero));
    }
    void CalculateCameraPosition()
    {
        MainCamera.transform.position = CameraTarget.position;
    }
    void CalculateMovement()
    {
        if (inputManeger.Input_Movement.y <= 0.2f)
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
            dir = direction,
            isSprinting = IsSprinting
        });
        verticalSpeed *= PlayerSettings.SpeedEffector;
        horizontalSpeed *= PlayerSettings.SpeedEffector;
        newMovementSpeed = Vector3.SmoothDamp(newMovementSpeed, new Vector3(verticalSpeed * inputManeger.Input_Movement.x * Time.deltaTime, 0, horizontalSpeed * inputManeger.Input_Movement.y * Time.deltaTime), ref newMovementSpeedVelocity, IsGrounded() ? PlayerSettings.MovementSmoothing : PlayerSettings.FallingSmoothing);
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
        if (inputManeger.IsLeaningLeft)
        {
            TargetLean = LeanAngle;
        }
        else if (inputManeger.IsLeaningRight)
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
        if (inputManeger.Input_Movement.y <= 0.2f)
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

    #region - HeadBob -

    void CalculateHeadBob()
    {
        if (!IsGrounded())
            return;

        if (inputManeger.Input_Movement != Vector2.zero)
        {
            HeadBobTimer += Time.deltaTime *
                (playerStance == PlayerStance.Crouch ? PlayerSettings.CrouchBobSpeed :
                playerStance == PlayerStance.Prone ? PlayerSettings.ProneBobSpeed :
                IsSprinting ? PlayerSettings.SprintBobSpeed : PlayerSettings.WalkBobSpeed);
            CameraTarget.localPosition = Vector3.up * (DefautYPos + Mathf.Sin(HeadBobTimer) *
                (playerStance == PlayerStance.Crouch ? PlayerSettings.CrouchBobAmount :
                playerStance == PlayerStance.Prone ? PlayerSettings.ProneBobAmount :
                IsSprinting ? PlayerSettings.SprintBobAmount : PlayerSettings.WalkBobAmount));
        }
        CameraTarget.localPosition = Vector3.Lerp(CameraTarget.localPosition, Vector3.up*DefautYPos, 0.2f);
    }


    #endregion

    #region - Gizmos -
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(feetTransfrom.position, PlayerSettings.IsGroundedRadius);
    }
    #endregion

}
