using UnityEngine;
using static scr_Models;



public class scr_WeaponController : MonoBehaviour
{
    private scr_CharacterController characterController;
    private bool IsInitialized;
    private Vector3 newWeaponRotation;
    private Vector3 newWeaponRotationVelocity;
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

        newWeaponRotation.y += Settings.SwayAmount * (Settings.SwayXInverted ?-characterController.Input_View.x : characterController.Input_View.x) * Time.deltaTime;  
        newWeaponRotation.x += Settings.SwayAmount * (Settings.SwayYInverted ? characterController.Input_View.y : -characterController.Input_View.y) * Time.deltaTime;
        //newWeaponRotation.x = Mathf.Clamp(newWeaponRotation.x, ViewClampYmin, ViewClampYmax);
        transform.localRotation = Quaternion.Euler(newWeaponRotation);


    }
}
