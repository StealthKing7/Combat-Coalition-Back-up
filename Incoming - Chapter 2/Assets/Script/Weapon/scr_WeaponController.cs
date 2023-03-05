using System.Collections.Generic;
using static scr_Models;
using System.Linq;
using UnityEngine;


public class scr_WeaponController : MonoBehaviour
{
    #region - Parameters -


    //Rotation
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
    private Transform MainCamera;
    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] GameObject BulletPrefab;
    [SerializeField] Transform BulletSpawn;
    [SerializeField] Transform SightTaregt;
    [SerializeField] Transform SwayObj;
    //Animation
    private float animatorSpeed;
    private bool IsSprinting;
    private bool IsGroundedTrigger;
    private float FallingDelay;
    private bool IsGrounded;
    //Setting
    [Header("Settings")]
    [SerializeField] WeaponSettingsModel Settings;
    //Breathing
    [Header("Weapon Sway")]
    [SerializeField] float SwayAmountA = 1;
    [SerializeField] float SwayAmountB = 2;
    [SerializeField] float SwayScale = 600;
    [SerializeField] float SwayLerpSpeed = 14;
    private float SwayTime;
    private Vector3 SwayPosition;
    [Header("Sight")]
    [SerializeField] float SightOffset;
    [SerializeField] float ADSTime;
    private Vector3 WeaponSwayPosition;
    private Vector3 WeaponSwayPositionVelocity;
    [HideInInspector]
    public bool isAiming;
    [Header("Shooting")]
    [SerializeField] float RateOfFire;
    [SerializeField] List<WeaponFireType> AllowedFireTypes;
    [SerializeField] WeaponFireType currentFireType;
    [HideInInspector]
    public bool IsShooting;

    #endregion

    #region - Start/Upadate -
    private void Start()
    {
        newWeaponRotation = transform.localRotation.eulerAngles;

        currentFireType = AllowedFireTypes.First();
    }
    private void Update()
    {
        CalculateWeaponRotation();
        SetWeaponAnimations();
        CalculateWeaponSway();
        CalculateAimingIn();
        CalculateShooting();
    }
    #endregion

    #region  - References -

    public void GetWeaponSpeed(float _speed)
    {
        animatorSpeed = _speed;
    }
    public void GetWeaponAnimationBool(bool _isSprinting)
    {
        IsSprinting = _isSprinting;
    }
    public void GetCamera(Transform cam)
    {
        MainCamera = cam;
    }
    public void GetIsGrounded(bool _IsGrounded)
    {
        IsGrounded = _IsGrounded;
    }
    public void GetMovement(Vector3 _Input_Movement)
    {
        Input_Movement = _Input_Movement;
    }
    public void GetView(Vector3 _Input_View)
    {
        Input_View = _Input_View;   
    }

    #endregion

    #region - AimingIn -
    void CalculateAimingIn()
    {
        var targetPosition = transform.position;
        if (isAiming)
        {
            targetPosition = MainCamera.position + (SwayObj.position - SightTaregt.position) + (MainCamera.transform.forward * SightOffset);
        }
        WeaponSwayPosition = SwayObj.position;
        WeaponSwayPosition = Vector3.SmoothDamp(WeaponSwayPosition, targetPosition, ref WeaponSwayPositionVelocity, ADSTime);
        SwayObj.position = WeaponSwayPosition + SwayPosition;
    }
    #endregion

    #region - Shooting -
    
    void CalculateShooting()
    {
        if (IsShooting)
        {
            Shoot();
            if(currentFireType== WeaponFireType.SemiAuto)
            {
                IsShooting = false;
            }
        }
    }
    void Shoot()
    {
        var bullet = Instantiate(BulletPrefab, BulletSpawn.position, Quaternion.identity);
    }

    #endregion

    #region  - Jumping -
    public void TriggerJump()
    {
        IsGroundedTrigger = false;
        animator.SetTrigger("Jump");

    }
    #endregion
    
    #region - Rotation - 
    void CalculateWeaponRotation()
    {
        TargetWeaponRotation.y += (isAiming ? Settings.SwayAmount / 2 : Settings.SwayAmount) * (Settings.SwayXInverted ? -Input_View.x : Input_View.x) * Time.deltaTime;
        TargetWeaponRotation.x += (isAiming ? Settings.SwayAmount / 2 : Settings.SwayAmount) * (Settings.SwayYInverted ? Input_View.y : -Input_View.y) * Time.deltaTime;
        TargetWeaponRotation.x = Mathf.Clamp(TargetWeaponRotation.x, -Settings.SwayClampX, Settings.SwayClampX);
        TargetWeaponRotation.y = Mathf.Clamp(TargetWeaponRotation.y, -Settings.SwayClampY, Settings.SwayClampY);
        TargetWeaponRotation.z = isAiming ? 0 : TargetWeaponRotation.y;
        TargetWeaponRotation = Vector3.SmoothDamp(TargetWeaponRotation, Vector3.zero, ref TargetWeaponRotationVelocity, Settings.SwayResetSmoothing);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, TargetWeaponRotation, ref newWeaponRotationVelocity, Settings.SwaySmoothing);
        TargetWeaponMovementRotation.z = (isAiming ? Settings.MovementSwayX / 2 : Settings.MovementSwayX) * (Settings.MovementSwayXInverted ? -Input_Movement.x : Input_Movement.x);
        TargetWeaponMovementRotation.x = (isAiming ? Settings.MovementSwayY / 2 : Settings.MovementSwayY) * (Settings.MovementSwayYInverted ? -Input_Movement.y : Input_Movement.y);
        TargetWeaponMovementRotation = Vector3.SmoothDamp(TargetWeaponMovementRotation, Vector3.zero, ref TargetWeaponMovementRotationVelocity, Settings.SwayResetSmoothing);
        newWeaponMovementRotation = Vector3.SmoothDamp(newWeaponMovementRotation, TargetWeaponMovementRotation, ref newWeaponMovementRotationVelocity, Settings.SwaySmoothing);
        transform.localRotation = Quaternion.Euler(newWeaponRotation + newWeaponMovementRotation);
    }
    #endregion

    #region - Animations - 
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
    #endregion

    #region - Sway -
    void CalculateWeaponSway()
    {
        var targetPos = Curve(SwayTime, SwayAmountA, SwayAmountB) / (isAiming ? SwayScale * 4 : SwayScale);
        SwayPosition = Vector3.Lerp(SwayPosition, targetPos, Time.smoothDeltaTime * SwayLerpSpeed);
        SwayTime += Time.deltaTime;
        if (SwayTime > 6.3f)
        {
            SwayTime = 0;
        }
        //SwayObj.localPosition = SwayPosition;

    }
    private Vector3 Curve(float Time, float A, float B)
    {
        return new Vector3(Mathf.Sin(Time), A * Mathf.Sin(B * Time + Mathf.PI));
    }
    #endregion
}
