using UnityEngine;
using static scr_Models;



public class scr_WeaponController : MonoBehaviour
{
    private scr_CharacterController characterController;
    private bool IsInitialized;
    private Vector3 newWeaponRotation;
    private Vector3 newWeaponRotationVelocity;
    private Vector3 TargetWeaponRotation;
    private Vector3 TargetWeaponRotationVelocity;
    private Vector3 newWeaponMovementRotation;
    private Vector3 newWeaponMovementRotationVelocity;
    private Vector3 TargetWeaponMovementRotation;
    private Vector3 TargetWeaponMovementRotationVelocity;
    [SerializeField] Animator animator;
    private float animatorSpeed;
    private bool IsSprinting;
    private bool IsGroundedTrigger;
    private float FallingDelay;
    private bool IsGrounded;
    [Header("Settings")]
    [SerializeField] WeaponSettingsModel Settings;


    private void Start()
    {
        newWeaponRotation = transform.localRotation.eulerAngles;
    }
    public void Initialize(scr_CharacterController _CharacterController)
    {
        characterController = _CharacterController;
        IsInitialized = true;
    }
    public void GetWeaponSpeed(float _speed)
    {
        animatorSpeed = _speed;
    }
    public void GetWeaponAnimationBool(bool _isSprinting)
    {
        IsSprinting = _isSprinting;
    }
    public void GetIsGrounded(bool _IsGrounded)
    {
        IsGrounded = _IsGrounded;
    }
    private void Update()
    {
        if (!IsInitialized)
        {
            return;
        }
        CalculateWeaponRotation();
        SetWeaponAnimations();


    }
    public void TriggerJump()
    {
        IsGroundedTrigger = false;
        animator.SetTrigger("Jump");

    }
    void CalculateWeaponRotation()
    {
        TargetWeaponRotation.y += Settings.SwayAmount * (Settings.SwayXInverted ? -characterController.Input_View.x : characterController.Input_View.x) * Time.deltaTime;
        TargetWeaponRotation.x += Settings.SwayAmount * (Settings.SwayYInverted ? characterController.Input_View.y : -characterController.Input_View.y) * Time.deltaTime;
        TargetWeaponRotation.x = Mathf.Clamp(TargetWeaponRotation.x, -Settings.SwayClampX, Settings.SwayClampX);
        TargetWeaponRotation.y = Mathf.Clamp(TargetWeaponRotation.y, -Settings.SwayClampY, Settings.SwayClampY);
        TargetWeaponRotation.z = TargetWeaponRotation.y;
        TargetWeaponRotation = Vector3.SmoothDamp(TargetWeaponRotation, Vector3.zero, ref TargetWeaponRotationVelocity, Settings.SwayResetSmoothing);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, TargetWeaponRotation, ref newWeaponRotationVelocity, Settings.SwaySmoothing);
        TargetWeaponMovementRotation.z = Settings.MovementSwayX * (Settings.MovementSwayXInverted ? -characterController.Input_Movement.x : characterController.Input_Movement.x);
        TargetWeaponMovementRotation.x = Settings.MovementSwayY * (Settings.MovementSwayYInverted ? -characterController.Input_Movement.y : characterController.Input_Movement.y);
        TargetWeaponMovementRotation = Vector3.SmoothDamp(TargetWeaponMovementRotation, Vector3.zero, ref TargetWeaponMovementRotationVelocity, Settings.SwayResetSmoothing);
        newWeaponMovementRotation = Vector3.SmoothDamp(newWeaponMovementRotation, TargetWeaponMovementRotation, ref newWeaponMovementRotationVelocity, Settings.SwaySmoothing);
        transform.localRotation = Quaternion.Euler(newWeaponRotation + newWeaponMovementRotation);
    }
    void SetWeaponAnimations()
    {
        if (IsGroundedTrigger)
        {
            FallingDelay = 0;
        }
        else
        {
            FallingDelay += Time.deltaTime;
        }
        if (IsGrounded && !IsGroundedTrigger && FallingDelay > 0.1f)
        {
            animator.SetTrigger("Land");
            IsGroundedTrigger = true;
        }
        else if (!IsGrounded && IsGroundedTrigger)
        {
            animator.SetTrigger("Falling");
            IsGroundedTrigger = false;
        }
        animator.SetBool("IsSprinting", IsSprinting);
        animator.SetFloat("WalkingSpeed", animatorSpeed);
    }
}
