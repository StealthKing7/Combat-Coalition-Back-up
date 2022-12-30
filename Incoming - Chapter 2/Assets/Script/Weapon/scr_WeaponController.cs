using UnityEngine;
using static scr_Models;



public class scr_WeaponController : MonoBehaviour
{
    private Vector3 Input_View;
    private Vector3 Input_Movement;
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
    [Header("Weapon Sway")]
    [SerializeField] Transform SwayObj;
    [SerializeField] float SwayAmountA = 1;
    [SerializeField] float SwayAmountB = 2;
    [SerializeField] float SwayScale = 600;
    [SerializeField] float SwayLerpSpeed = 14;
    private float SwayTime;
    private Vector3 SwayPosition;
    private void Start()
    {
        newWeaponRotation = transform.localRotation.eulerAngles;
    }

    #region References

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
    public  void GetMovement(Vector3 _Input_Movement)
    {
        Input_Movement = _Input_Movement;
    }
    public  void GetView(Vector3 _Input_View)
    {
        Input_View = _Input_View;
    }

    #endregion

    private void Update()
    {
        CalculateWeaponRotation();
        SetWeaponAnimations();
        CalculateWeaponSway();
    }
    public void TriggerJump()
    {
        IsGroundedTrigger = false;
        animator.SetTrigger("Jump");

    }
    void CalculateWeaponRotation()
    {
        TargetWeaponRotation.y += Settings.SwayAmount * (Settings.SwayXInverted ? -Input_View.x : Input_View.x) * Time.deltaTime;
        TargetWeaponRotation.x += Settings.SwayAmount * (Settings.SwayYInverted ? Input_View.y : -Input_View.y) * Time.deltaTime;
        TargetWeaponRotation.x = Mathf.Clamp(TargetWeaponRotation.x, -Settings.SwayClampX, Settings.SwayClampX);
        TargetWeaponRotation.y = Mathf.Clamp(TargetWeaponRotation.y, -Settings.SwayClampY, Settings.SwayClampY);
        TargetWeaponRotation.z = TargetWeaponRotation.y;
        TargetWeaponRotation = Vector3.SmoothDamp(TargetWeaponRotation, Vector3.zero, ref TargetWeaponRotationVelocity, Settings.SwayResetSmoothing);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, TargetWeaponRotation, ref newWeaponRotationVelocity, Settings.SwaySmoothing);
        TargetWeaponMovementRotation.z = Settings.MovementSwayX * (Settings.MovementSwayXInverted ? -Input_Movement.x : Input_Movement.x);
        TargetWeaponMovementRotation.x = Settings.MovementSwayY * (Settings.MovementSwayYInverted ? -Input_Movement.y : Input_Movement.y);
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
    void CalculateWeaponSway()
    {
        var targetPos = Curve(SwayTime, SwayAmountA, SwayAmountB) / SwayScale;
        SwayPosition = Vector3.Lerp(SwayPosition, targetPos, Time.smoothDeltaTime * SwayLerpSpeed);
        SwayTime += Time.deltaTime;
        if (SwayTime > 6.3f)
        {
            SwayTime = 0;
        }
        SwayObj.localPosition = SwayPosition;

    }
    private Vector3 Curve(float Time, float A, float B)
    {
        return new Vector3(Mathf.Sin(Time), A * Mathf.Sin(B * Time + Mathf.PI));
    }
}
