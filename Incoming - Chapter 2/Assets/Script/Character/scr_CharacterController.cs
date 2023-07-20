using UnityEngine;
using static scr_Models;
public class scr_CharacterController : MonoBehaviour
{
    #region - Parameters -

    scr_InputManeger inputManeger;
    Vector3 NewCameraRotation;
    Vector3 NewCharacterRotation;
    CharacterController characterController;
    [Header("References")]
    [SerializeField] Transform CameraHolder;
    [SerializeField] Transform MainCamera;
    [SerializeField] Transform feetTransfrom;
    [SerializeField] UnityEngine.UI.Text fpsText;
    [Header("Player Settings")]
    [SerializeField] PlayerSettingModel PlayerSettings;
    [SerializeField] float ViewClampYmin = -70;
    [SerializeField] float ViewClampYmax = 80;
    [SerializeField] LayerMask PlayerMask;
    [SerializeField] LayerMask GroundMask;
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
    [SerializeField] scr_WeaponController currentWeapon;
    [SerializeField] float WeaponAnimationSpeed;
    [Header("Leaning")]
    [SerializeField] Transform LeanPiviot;
    [SerializeField] float LeanAngle;
    [SerializeField] float LeanSmoothing;
    private float CurrentLean;
    private float TargetLean;
    private float TargetLeanVelocity;
    [Header("Aiming")]
    private bool isAiming;
    float frameRate;
    float timer;
    #endregion

    
    #region - Awake/Update -
    private void Awake()
    {
        NewCharacterRotation = transform.localRotation.eulerAngles;
        NewCameraRotation = CameraHolder.localRotation.eulerAngles;
        characterController = GetComponent<CharacterController>();
        CameraHeight = CameraHolder.localPosition.y;
        currentWeapon.GetCamera(MainCamera);
    }
    private void Start()
    {
        inputManeger = scr_InputManeger.Instance;
        inputManeger.SetWeapon(currentWeapon);
        scr_InputManeger.Instance.Jump += Jump;
        inputManeger.Crouch += Crouch;
        inputManeger.Prone += Prone;
        inputManeger.ToggleSprint += ToggleSprint;
        inputManeger.StopSprint += StopSprint;
        inputManeger.AimingInPressed += AimingInPressed;
        inputManeger.AimingInReleased += AimingInReleased;
    }
    private void Update()
    {

        fpsText.text = frameRate + " fps";
        if (timer > 1f)
        {
            frameRate = (int)(1f / Time.unscaledDeltaTime);
            timer = 0f;
        }
        else
        {
            timer += Time.deltaTime;
        }
        IsGrounded();
        currentWeapon.GetIsGrounded(IsGrounded());
        IsFalling();
        CalculateView();
        CalculateMovement();
        CalculateJump();
        CalculateStance();
        CalculateLeaning();
        CalcutaleAiming();
    }

    #endregion 

    
    #region - Aiming In -

    void AimingInPressed()
    {
        isAiming = true;
    }
    void AimingInReleased()
    {
        isAiming = false;
    }
    void CalcutaleAiming()
    {
        if (!currentWeapon)
        {
            return;
        }
        currentWeapon.isAiming = isAiming;
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
        NewCharacterRotation.y += (isAiming ? PlayerSettings.AimSensitivityEffector : PlayerSettings.ViewXSencitivity) * (PlayerSettings.ViewXInverted ? -inputManeger.GetInput_View().x : inputManeger.GetInput_View().x) * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(NewCharacterRotation);
        NewCameraRotation.x += (isAiming ? PlayerSettings.AimSensitivityEffector : PlayerSettings.ViewXSencitivity) * (PlayerSettings.ViewYInverted ? inputManeger.GetInput_View().y : -inputManeger.GetInput_View().y) * Time.deltaTime;
        NewCameraRotation.x = Mathf.Clamp(NewCameraRotation.x, ViewClampYmin, ViewClampYmax);
        CameraHolder.localRotation = Quaternion.Euler(NewCameraRotation);
        currentWeapon.GetView(inputManeger.GetInput_View());
    }
    void CalculateMovement()
    {
        if (inputManeger.GetInput_Movement().y <= 0.2f)
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
        else if (isAiming)
        {
            PlayerSettings.SpeedEffector = PlayerSettings.AimSpeedEffector;
        }
        else
        {
            PlayerSettings.SpeedEffector = 1;
        }

        WeaponAnimationSpeed = characterController.velocity.magnitude / (PlayerSettings.WalkingForwardSpeed * PlayerSettings.SpeedEffector);
        if (WeaponAnimationSpeed > 1)
        {
            WeaponAnimationSpeed = 1f;
        }
        currentWeapon.GetWeaponSpeed(WeaponAnimationSpeed);
        verticalSpeed *= PlayerSettings.SpeedEffector;
        horizontalSpeed *= PlayerSettings.SpeedEffector;
        newMovementSpeed = Vector3.SmoothDamp(newMovementSpeed, new Vector3(verticalSpeed * inputManeger.GetInput_Movement().x * Time.deltaTime, 0, horizontalSpeed * inputManeger.GetInput_Movement().y * Time.deltaTime), ref newMovementSpeedVelocity, IsGrounded() ? PlayerSettings.MovementSmoothing : PlayerSettings.FallingSmoothing);
        currentWeapon.GetMovement(inputManeger.GetInput_Movement());
        var movementSpeed = transform.TransformDirection(newMovementSpeed);
        if (PlayerGravity > GravityMin)
        {

            PlayerGravity -= GravityAmount * Time.deltaTime;
        }

        if (PlayerGravity < -0.1f && IsGrounded())
        {
            PlayerGravity = -0.1f;
        }

        movementSpeed.y += PlayerGravity;
        movementSpeed += JumpingForce * Time.deltaTime;
        characterController.Move(movementSpeed);
    }
    #endregion


    #region - Leaning -
    void CalculateLeaning()
    {
        if (inputManeger.GetIsLeaningLeft())
        {
            TargetLean = LeanAngle;
        }
        else if (inputManeger.GetIsLeaningRight())
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
        currentWeapon.TriggerJump();
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
        if (IsFalling())
        {
            playerStance = PlayerStance.Stand;
        }

        CameraHeight = Mathf.SmoothDamp(CameraHolder.localPosition.y, currentStance.CameraHeight, ref CameraHeightVelocity, PlayerStanceSmoothing);
        CameraHolder.localPosition = new Vector3(CameraHolder.localPosition.x, CameraHeight, CameraHolder.localPosition.z);

        characterController.height = Mathf.SmoothDamp(characterController.height, currentStance.StanceCollider.height, ref StanceCapsuleHieghtVelocity, PlayerStanceSmoothing);
        characterController.center = Vector3.SmoothDamp(characterController.center, currentStance.StanceCollider.center, ref StanceCapsuleCenterVeloctiy, PlayerStanceSmoothing);
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
    #endregion


    #region - Sprinting -
    void ToggleSprint()
    {
        if (inputManeger.GetInput_Movement().y <= 0.2f)
        {
            IsSprinting = false;
            return;
        }
        IsSprinting = !IsSprinting;
        currentWeapon.GetWeaponAnimationBool(IsSprinting);
    }
    void StopSprint()
    {
        if(PlayerSettings.SprintHold)
        IsSprinting = false;
        currentWeapon.GetWeaponAnimationBool(IsSprinting);
    }
    #endregion


    #region - Gizmos -
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(feetTransfrom.position, PlayerSettings.IsGroundedRadius);
    }
    #endregion

}
