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
    public Transform Bullet;
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
    public int Iteration;
    public Vector3 WindVector;
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
