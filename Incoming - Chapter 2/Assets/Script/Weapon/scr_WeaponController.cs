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
    [Header("Settings")]
    public WeaponSettingsModel Settings;


    private void Start()
    {
        newWeaponRotation = transform.localRotation.eulerAngles;
    }

    public void Initialize(scr_CharacterController _CharacterController)
    {
        characterController = _CharacterController;
        IsInitialized = true;
    }
    private void Update()
    {
        if (!IsInitialized)
        {
            return;
        }

        TargetWeaponRotation.y += Settings.SwayAmount * (Settings.SwayXInverted ? -characterController.Input_View.x : characterController.Input_View.x) * Time.deltaTime;
        TargetWeaponRotation.x += Settings.SwayAmount * (Settings.SwayYInverted ? characterController.Input_View.y : -characterController.Input_View.y) * Time.deltaTime;
        TargetWeaponRotation.x = Mathf.Clamp(TargetWeaponRotation.x, -Settings.SwayClampX, Settings.SwayClampX);
        TargetWeaponRotation.y = Mathf.Clamp(TargetWeaponRotation.y, -Settings.SwayClampY, Settings.SwayClampY);
        TargetWeaponRotation.z = TargetWeaponRotation.y;
        TargetWeaponRotation = Vector3.SmoothDamp(TargetWeaponRotation, Vector3.zero, ref TargetWeaponRotationVelocity, Settings.SwayResetSmoothing);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, TargetWeaponRotation, ref newWeaponRotationVelocity, Settings.SwaySmoothing);
        transform.localRotation = Quaternion.Euler(newWeaponRotation);


    }
}
