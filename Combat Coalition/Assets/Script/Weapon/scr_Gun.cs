//using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static scr_Models;
using static UnityEngine.GraphicsBuffer;

public class scr_Gun : scr_BaseWeapon
{
    #region - Parameters - 
    public Vector3 TestingVec;
    private Ray ray;
    private Dictionary<AttachmentTypes, scr_Attachment_SO> CurrentAttachments = new Dictionary<AttachmentTypes, scr_Attachment_SO>();
    public event  EventHandler<OnShootEventArgs> OnAmmoChange;
    public class OnShootEventArgs : EventArgs { public float CurrentAmmo; } 
    private List<BulletWithTarget> bulletWithDirs = new List<BulletWithTarget>();
    [SerializeField] AttachmentsPoints[] _AttachmentsPoints;
    [SerializeField] Transform SightTarget;
    [SerializeField] Transform BulletSpawn;
    private float RecoilTime;
    private float FOVFloatVelocity;
    private Vector3 GunAimPosition;
    private Vector3 GunAimPositionVelocity;
    private Vector3 RecoilTargetPos;
    private Vector3 RecoilTargetPosVelocity;
    private Vector3 RecoilTargetRot;
    private Vector3 RecoilTargetRotVelocity;
    private Vector3 HitPoint;
    private Vector3 CameraShakeVec;
    private Vector3 CameraRecoilVec;
    public Vector3 CamRecoil { get; private set; }
    private float CurrentAmmo = 0f;
    private scr_GunSO _GunSO;
    private scr_InputManeger inputManeger;
    #endregion


    #region  - Shoot -
    public override void Execute()
    {
        if (CurrentAmmo == 0f)
        {
            StartCoroutine(Reload());
            return;
        }
        Shoot();
    }
    void CalculateShooting()
    {
        bulletWithDirs.RemoveAll(b => b.scr_Bullet == null);
        ray = holder.Cam().ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        if(Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, holder.GetWeaponController().BulletIgnoreLayer))
        {
            HitPoint = hitInfo.point;
        }
        foreach(var bullet in  bulletWithDirs)
        {
            bullet.scr_Bullet.OnHit += Bullet_OnHit;
        }
    }

    private void Bullet_OnHit(object sender, scr_Bullet.OnHitEventArgs e)
    {
        Debug.Log(e.hitObject.name);
        holder.GetWeaponController().debugobj.position = e.hitObject.transform.position;
    }

    void Shoot()
    {
        CurrentAmmo--;
        OnAmmoChange?.Invoke(this, new OnShootEventArgs { CurrentAmmo = CurrentAmmo });
        RecoilTime = Time.deltaTime;
        bulletWithDirs.Add(new BulletWithTarget { BulletTarget = HitPoint, scr_Bullet = Instantiate(_GunSO.Bullet) });
        bulletWithDirs.RemoveAll(b => b.scr_Bullet == null);
        bulletWithDirs.ForEach(b =>
        {
            if (b.scr_Bullet.IsInitialized()) return;
            b.scr_Bullet.SetStartValues(_GunSO, BulletSpawn.position, (b.BulletTarget - BulletSpawn.position).normalized);
        });
        }
    #endregion

    #region - Awake/Start/Update/FixedUpdate -

    private void Start()
    {
        inputManeger = scr_InputManeger.Instance;
        inputManeger.AimingInPressed += AimingInPressed;
        inputManeger.AimingInReleased += AimingInReleased;
        inputManeger.Reload += ReloadEvent;
        holder.GetWeaponController().OnWeaponEquiped += Scr_Gun_OnWeaponEquiped;
        _GunSO = GetScr_WeaponSO() as scr_GunSO;
        CurrentAmmo = _GunSO.MaxAmmo;
    }
    private void Scr_Gun_OnWeaponEquiped(object sender, scr_WeaponController.OnWeaponEquipedEventArgs e)
    {
        LoadAttachments(e.attachment_SO);
    }
    private void Update()
    {
        CalculateShooting();
        CalculateAimingIn();
        CalculateRecoil();
    }
    #endregion
    #region - Reload -
    void ReloadEvent()
    {
        StartCoroutine(Reload());
    }
    IEnumerator Reload()
    {
        //if (CurrentAmmo == _GunSO.MaxAmmo) yield break;
        yield return new WaitForSeconds(_GunSO.ReloadTime);
        CurrentAmmo = _GunSO.MaxAmmo;
        OnAmmoChange?.Invoke(this, new OnShootEventArgs { CurrentAmmo = CurrentAmmo });
    }
    #endregion
    #region - AimingIn -
    void CalculateAimingIn()
    {
        var AimTargetPos = Vector3.zero;
        var targetFOV = 60f;
        if (IsAiming)
        {
            AimTargetPos = holder.Cam().transform.position - holder.GetWeaponParent().position + (transform.position - SightTarget.position) + (holder.Cam().transform.forward * _GunSO.SightOffset);
            targetFOV = _GunSO.FOV;
        }
        holder.Cam().fieldOfView = Mathf.SmoothDamp(holder.Cam().fieldOfView, targetFOV, ref FOVFloatVelocity, _GunSO.ADSTime);
        GunAimPosition = transform.localPosition;
        GunAimPosition = Vector3.SmoothDamp(GunAimPosition, transform.InverseTransformVector(AimTargetPos + (CurrentAttachments.TryGetValue(AttachmentTypes.Sight,out scr_Attachment_SO _sight)?(_sight as scr_Sight_SO).SightOffset : Vector3.zero)), ref GunAimPositionVelocity, _GunSO.ADSTime);
        transform.localPosition = GunAimPosition;
    }
    void AimingInPressed()
    {
        IsAiming = true;
    }
    void AimingInReleased()
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
        CameraRecoilVec += new Vector3(-(IsAiming ? _GunSO.CameraRecoilX/2 : _GunSO.CameraRecoilX), UnityEngine.Random.Range(-_GunSO.CameraRecoilY, _GunSO.CameraRecoilY), 0); 
        CameraShakeVec += new Vector3(UnityEngine.Random.Range(-(IsAiming? _GunSO.CameraShake/2 : _GunSO.CameraShake), 0), 0, 0);
    }
    void CalculateRecoil()
    {
        RecoilTargetPos = Vector3.zero;
        RecoilTargetRot = Vector3.zero;
        CameraShakeVec = Vector3.zero;
        if (RecoilTime > 0)
        {
            float fraction = RecoilTime / _GunSO.RecoilSmoothing;
            RecoilTime += Time.deltaTime;
            if (RecoilTime > _GunSO.RecoilSmoothing)
            {
                RecoilTime = 0;
                fraction = 0;
            }
            Recoil(fraction);
        }
        Vector3 Recoilrotation = Vector3.zero;
        Recoilrotation = Vector3.SmoothDamp(Recoilrotation, RecoilTargetRot, ref RecoilTargetRotVelocity, RecoilTime * Time.deltaTime);
        Vector3 RecoilPos = Vector3.SmoothDamp(transform.localPosition, RecoilTargetPos, ref RecoilTargetPosVelocity, RecoilTime* Time.deltaTime);
        Vector3 camShake = Vector3.zero;
        camShake = Vector3.SmoothDamp(camShake, CameraShakeVec, ref RecoilTargetRotVelocity, 1);
        CameraRecoilVec = Vector3.Lerp(CameraRecoilVec, Vector3.zero, 2 * Time.deltaTime);
        CamRecoil = Vector3.Slerp(CamRecoil, CameraRecoilVec, 6 * Time.fixedDeltaTime);
        transform.localPosition += RecoilPos;
        transform.localRotation = Quaternion.Euler(Recoilrotation);
        holder.Cam().transform.localRotation = Quaternion.Euler(camShake);
    }
    #endregion
    void LoadAttachments(List<scr_Attachment_SO> attachment_SOs)
    {
        for (int i = 0; i < _AttachmentsPoints.Length; i++)
        {
            var validAttachment = attachment_SOs.Find(e => e.AttachmentType == _AttachmentsPoints[i].AttachmentType);
            if (validAttachment != null)
            {
                Instantiate(validAttachment.Model, _AttachmentsPoints[i].Point);
                CurrentAttachments.Add(validAttachment.AttachmentType, validAttachment);
            }
        }
    }
}
