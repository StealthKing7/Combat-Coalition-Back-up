using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Gun : scr_BaseWeapon
{
    #region - Parameters - 
    private Ray ray;
    private List<scr_Bullet> bullets = new List<scr_Bullet>();
    private GameObject TargetObj;
    [SerializeField] Transform BulletSpawn;
    private float RecoilTime;
    private Vector3 WeaponSwayPosition;
    private Vector3 WeaponSwayPositionVelocity;
    private float FOVFloatVelocity;
    public Vector3 RecoilTargetPos;
    private Vector3 RecoilTargetPosVelocity;
    public Vector3 RecoilTargetRot;
    private Vector3 RecoilTargetRotVelocity;
    private Vector3 BulletTargetPos;
    private scr_GunSO _GunSO;
    #endregion


    #region  - Shoot -
    public override void Execute()
    {
        Shoot();
    }
    void CalculateShooting()
    {
        bullets.RemoveAll(b => b == null);
        ray = holder.Cam().ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        if(Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, holder.GetWeaponController().BulletIgnoreLayer))
        {
            holder.GetWeaponController().debugobj.position = BulletTargetPos = hitInfo.point;
        }
        /*foreach (var bullet in bullets)
        {
            
        }*/

    }
    void Shoot()
    {
        RecoilTime = Time.deltaTime;
        Vector3 dir = (BulletTargetPos - BulletSpawn.position).normalized;
        bullets.Add(Instantiate(_GunSO.Bullet));
        bullets.RemoveAll(b => b == null);
        if (bullets.Count > 0)
        {
            bullets.ForEach(b => b.SetStartValues(_GunSO, BulletSpawn.position, dir));
        }
    }
    #endregion

    #region - Start/Update/FixedUpdate -
    private void Start()
    {
        if (holder == null) return;

        scr_InputManeger.Instance.AimingInPressed += AimingInPressed;
        scr_InputManeger.Instance.AimingInReleased += AimingInReleased;
        _GunSO = GetScr_WeaponSO() as scr_GunSO;  
    }
    private void Update()
    {
        if (holder == null) return;
        CalculateShooting();
        CalculateAimingIn();
        CalculateRecoil();
    }
    #endregion

    public void Reload()
    {
        
    }

    #region - AimingIn -
    void CalculateAimingIn()
    {
        var AimTargetPos = Vector3.zero;
        var targetFOV = 60f;
        if (IsAiming && holder.HasWeapon())
        {
            AimTargetPos = _GunSO.SightPos;
            targetFOV = _GunSO.FOV;
        }
        holder.Cam().fieldOfView = Mathf.SmoothDamp(holder.Cam().fieldOfView, targetFOV, ref FOVFloatVelocity, _GunSO.ADSTime);
        WeaponSwayPosition = transform.localPosition;
        WeaponSwayPosition = Vector3.SmoothDamp(WeaponSwayPosition, AimTargetPos, ref WeaponSwayPositionVelocity, _GunSO.ADSTime);
        transform.localPosition = WeaponSwayPosition;
    }
    public void AimingInPressed()
    {
        IsAiming = true;
    }
    public void AimingInReleased()
    {
        IsAiming = false;
    }

    #endregion

    #region - Recoil - 
    void Recoil(float fraction)
    {
        float RecoilPosY = _GunSO.KickBackY.Evaluate(fraction) * _GunSO.KickBackYMultiplier;
        float RecoilPosZ = _GunSO.KickBackZ.Evaluate(fraction) * _GunSO.KickBackZMultiplier;

        float RecoilRotY = _GunSO.RotationY.Evaluate(fraction,0.5f) * _GunSO.RotationYMultiplier; 
        float RecoilRotX = _GunSO.RotationX.Evaluate(fraction) * _GunSO.RotationXMultiplier;
        RecoilTargetPos = new Vector3(0, RecoilPosY, RecoilPosZ);
        RecoilTargetRot =  new Vector3(RecoilRotX, RecoilRotY, 0);
    }
    void CalculateRecoil()
    {
        RecoilTargetPos = Vector3.zero;
        RecoilTargetRot = Vector3.zero;
        if (RecoilTime > 0)
        {
            float fraction = RecoilTime / _GunSO.FireRate;
            RecoilTime += Time.deltaTime;
            if (RecoilTime > _GunSO.FireRate)
            {
                RecoilTime = 0;
                fraction = 0;
            }
            Recoil(fraction);
        }
        Vector3 Recoilrotation = Vector3.zero;
        Recoilrotation = Vector3.SmoothDamp(Recoilrotation, RecoilTargetRot, ref RecoilTargetRotVelocity, RecoilTime * Time.deltaTime);
        Vector3 RecoilPos = Vector3.SmoothDamp(transform.localPosition, RecoilTargetPos, ref RecoilTargetPosVelocity, RecoilTime* Time.deltaTime);
        //holder.Cam().transform.localRotation = Quaternion.Euler(Vector3.SmoothDamp(holder.Cam().transform.localRotation.eulerAngles, RecoilTargetRot, ref RecoilTargetRotVelocity, RecoilTime));
        transform.localPosition += RecoilPos;
        transform.localRotation = Quaternion.Euler(Recoilrotation);
    }
    #endregion
}
