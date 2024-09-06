using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static scr_Models;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu()]
public class scr_GunSO : scr_WeaponSO
{
    [Header("Sight")]
    public Vector3 SightPos;
    public float SightOffset;
    public float FOV;
    public float ADSTime;
    public RectTransform Rectile;
    [Header("Attachments")]
    public List<scr_Attachment_SO> DefaultAttachments = new List<scr_Attachment_SO>();
    public scr_ValidAttachmentsSO ValidAttachments;
    [Header("Shooting Properties")]
    public scr_Bullet Bullet;
    public float BulletSpeed;
    public List<GunFireType> AllowedFireTypes;
    public float FireRate = 10f;
    public float BulletSpread;
    [Header("Audio")]
    public EventReference GunShots;
    public EventReference TriggerPressed;
    [Header("Reloading Properties")]
    public float MaxAmmo = 30f;
    public float ReloadTime = 3f;
    [Header("Bullet Drop Properties")]
    //The initial speed [m/s]
    //Mass [kg]
    public float m = 0.2f;
    //Radius [m]
    public float r = 0.05f;
    //Coefficients, which is a value you can't calculate - you have to simulate it in a wind tunnel
    //and they also depends on the speed, so we pick some average value
    //Drag coefficient (Tesla Model S has the drag coefficient 0.24)
    public float DragCoefficient = 0.5f;
    //Lift coefficient
    public float LiftCoefficient = 0.5f;
    //External data (shouldn't maybe be here but is easier in this tutorial)
    //Wind speed [m/s]
    public Vector3 windSpeedVector = new Vector3(0f, 0f, 0f);
    //The density of the medium the bullet is travelling in, which in this case is air at 15 degrees [kg/m^3]
    public float rho = 1.225f;
    [Header("Recoil")]
    //Rotation
    public AnimationCurve RotationX;
    public float RotationXStrength;
    //Position
    public AnimationCurve KickBackZ;
    public float KickBackZMultiplier;
    public AnimationCurve KickBackY;
    public float KickBackYMultiplier;

    public float RecoilSmoothing;
    [Header("Camera Recoil")]
    public Vector3 CamRecoil;
    public float CamRecoilIncrement;
    public float CameraReturnSpeed;
    public float CameraRecoilScale;
    public float Snapiness;
    public float CameraShake;
    public float CameraShakeSmoothing;
}
