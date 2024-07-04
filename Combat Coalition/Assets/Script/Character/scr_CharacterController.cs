using System;
using UnityEngine;
using static scr_Models;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Properties;
public class scr_CharacterController : MonoBehaviour
{

    #region - Parameters -
    //Private
    private CharacterController characterController;
    private Vector3 StanceCapsuleCenterVeloctiy;
    private Vector3 CurrentCharacterRotation;
    private Vector3 currentCamRot;
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
    private Vector3 CurrentPos;
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
        Collider[] results = new Collider[1];
        return Physics.OverlapSphereNonAlloc(feetTransfrom.position, PlayerSettings.IsGroundedRadius, results, GroundMask) > 0;
    }
    bool IsFalling()
    {
        return (!IsGrounded() && characterController.velocity.magnitude >= PlayerSettings.IsFallingSpeed);
    }
    #endregion


    #region -View/Movement-
    void CalculateView()
    {
        NativeArray<float3> CameraRotation = new NativeArray<float3>(1, Allocator.TempJob);
        NativeArray<float3> CharacterRotation = new NativeArray<float3>(1, Allocator.TempJob);
        JobHandle CamHandle = ViewCamJobHandle(CameraRotation,CharacterRotation ,out ViewJob _viewCameraJob);
        CamHandle.Complete();
        currentCamRot = _viewCameraJob.CameraRotation[0];
        CameraHolder.localRotation = Quaternion.Euler(_viewCameraJob.CameraRotation[0]);
        CurrentCharacterRotation = _viewCameraJob.CharacterRotaion[0];
        Debug.Log(_viewCameraJob.CharacterRotaion[0]);
        transform.localRotation = Quaternion.Euler(_viewCameraJob.CharacterRotaion[0]);
        CharacterRotation.Dispose();
        CameraRotation.Dispose();
    }
    JobHandle ViewCamJobHandle(NativeArray<float3> CameraRotation,NativeArray<float3> characterRotation ,out ViewJob _viewCamera)
    {
        CameraRotation[0] = currentCamRot;
        characterRotation[0] = CurrentCharacterRotation;
        ViewJob viewCameraJob = new ViewJob()
        {
            Input_View = InputManeger.Input_View,
            CameraRotation = CameraRotation,
            CharacterRotaion = characterRotation,
            IsAiming = WeaponController.GetWeapon().IsAiming,
            ViewClampYmax = ViewClampYmax,
            ViewClampYmin = ViewClampYmin,
            IsGrounded = IsGrounded(),
            PlayerSettings = PlayerSettings,
            deltaTime = Time.deltaTime,
            
        };
        _viewCamera = viewCameraJob;
        return viewCameraJob.Schedule(1, 1);
    }
    //ViewJob(Burst)
    [BurstCompile]
    public struct ViewJob : IJobParallelFor
    {
        public PlayerSettingModel PlayerSettings;
        public bool IsAiming;
        public bool IsGrounded;
        public float3 NewCameraRotation;
        public float3 NewCharacterRotation;
        public float ViewClampYmin;
        public float ViewClampYmax;
        public float2 Input_View;
        public float3 CamRecoil;
        public float deltaTime;
        public NativeArray<float3> CharacterRotaion;
        public NativeArray<float3> CameraRotation;
        public void Execute(int index)
        {
            float3 charRot = CharacterRotaion[0];
            NewCharacterRotation.y += (IsAiming ? PlayerSettings.AimSensitivityEffector : PlayerSettings.ViewXSencitivity) * (PlayerSettings.ViewXInverted ? -Input_View.x : Input_View.x) * deltaTime;
            charRot += NewCharacterRotation;
            CharacterRotaion[0] = charRot;
            float3 cameraRot = CameraRotation[0];
            NewCameraRotation.x += (IsAiming ? PlayerSettings.AimSensitivityEffector : PlayerSettings.ViewXSencitivity) * (PlayerSettings.ViewYInverted ? Input_View.y : -Input_View.y) * deltaTime;
            cameraRot += NewCameraRotation; 
            cameraRot.x = math.clamp(cameraRot.x, ViewClampYmin, ViewClampYmax);
            CameraRotation[0] = cameraRot;
        }
    }
    void CalculateMovement()
    {
        NativeArray<float3> pos = new NativeArray<float3>(1, Allocator.TempJob);
        JobHandle handle = MoveHandle(pos,out MovementJob movementJob);
        handle.Complete();
        characterController.Move(movementJob.position[0]);
        pos.Dispose();

        var direction = transform.InverseTransformDirection(characterController.velocity);
        CharacterMovementAnimationEvent?.Invoke(this, new CharacterMovementAnimationEventArgs
        {
            dir = direction.normalized,
            isSprinting = IsSprinting
        });
    }
    JobHandle MoveHandle(NativeArray<float3> pos,out MovementJob _movementJob)
    {
        pos[0] = transform.position;
        MovementJob movementJob = new MovementJob()
        {
            position = pos,
            velocity = characterController.velocity,
            Input_Movement = InputManeger.Input_Movement,
            DeltaTime = Time.deltaTime,
            IsSprinting = IsSprinting,
            PlayerSettings = PlayerSettings,
            IsGrounded = IsGrounded(),
            IsFalling = IsFalling(),
            AnimationSpeed = AnimationSpeed,
            playerStance = playerStance,
            PlayerGravity = PlayerGravity,
            IsAiming = WeaponController.GetWeapon().IsAiming,
            newMovementSpeed = newMovementSpeed,
            newMovementSpeedVelocity = newMovementSpeedVelocity,
            GravityMin = GravityMin,
            JumpingForce = JumpingForce,
        };
        _movementJob = movementJob;
        return movementJob.Schedule(new TransformAccessArray(new Transform[1] { transform }));
    }
    //MovementJob(Burst)
    [BurstCompile]
    public struct MovementJob : IJobParallelForTransform
    {
        public float2 Input_Movement;
        public float DeltaTime;
        public bool IsSprinting;
        public PlayerSettingModel PlayerSettings;
        public bool IsGrounded;
        public bool IsFalling;
        public float AnimationSpeed;
        public float PlayerGravity;
        public PlayerStance playerStance;
        public float3 newMovementSpeed;
        public float3 newMovementSpeedVelocity;
        public bool IsAiming;
        public float3 velocity;
        public float GravityMin;
        public float3 JumpingForce;
        public NativeArray<float3> position;
        public void Execute(int index, TransformAccess transform)
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
            if (!IsGrounded)
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
            else if (IsAiming)
            {
                PlayerSettings.SpeedEffector = PlayerSettings.AimSpeedEffector;
            }
            else
            {
                PlayerSettings.SpeedEffector = 1;
            }
            AnimationSpeed = math.length(velocity) / (PlayerSettings.WalkingForwardSpeed * PlayerSettings.SpeedEffector);
            if (AnimationSpeed > 1)
            {
                AnimationSpeed = 1;
            }
            verticalSpeed *= PlayerSettings.SpeedEffector;
            horizontalSpeed *= PlayerSettings.SpeedEffector;
            //SmoothDamp Pending 
            newMovementSpeed = new float3(verticalSpeed * Input_Movement.x * DeltaTime, 0, horizontalSpeed * Input_Movement.y * DeltaTime);
            float3 movementSpeed = scr_Models.LocalToWorld(transform.localToWorldMatrix, newMovementSpeed);
            if (PlayerGravity < -0.1f && IsGrounded)
            {
                PlayerGravity = -0.1f;
            }
            if (PlayerGravity > GravityMin)
            {
                PlayerGravity += (GravityVec.y * (IsFalling ? PlayerSettings.GravityMultiplierFalling : PlayerSettings.GravityMultiplierJump)) * DeltaTime;
            }
            movementSpeed.y += PlayerGravity;
            movementSpeed += JumpingForce * DeltaTime;
            position[index] = movementSpeed;
        }
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
