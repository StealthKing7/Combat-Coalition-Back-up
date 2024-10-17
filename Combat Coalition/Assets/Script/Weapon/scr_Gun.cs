//using System;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static scr_Models;

public class scr_Gun : scr_BaseWeapon
{
    #region - Parameters - 
    private Ray ray;
    private Dictionary<AttachmentTypes, scr_Attachment_SO> CurrentAttachments = new Dictionary<AttachmentTypes, scr_Attachment_SO>();
    public event EventHandler<OnShootEventArgs> OnShoot;
    public event EventHandler<OnReloadEventArgs> OnReload;
    public event EventHandler<Aim_ReloadEventArgs> Aim_Reload;
    public class Aim_ReloadEventArgs : EventArgs { public float Weight; }
    public class OnShootEventArgs : EventArgs { public float CurrentAmmo; }
    public class OnReloadEventArgs : EventArgs { public float CurrentAmmo; public bool isAiming; public float delay; }
    private List<BulletsWithPath> BulletWithPath = new List<BulletsWithPath>();
    [SerializeField] AttachmentsPoints[] _AttachmentsPoints;
    [SerializeField] Transform SightTarget;
    [SerializeField] Transform BulletSpawn;
    private float RecoilTime;
    private float FOVFloatVelocity;
    private Vector3 GunAimPosition;
    private Vector3 GunAimPositionVelocity;
    private Vector3 RecoilPos;
    private Vector3 RecoilTargetPosVelocity;
    private Vector3 RecoilRot;
    private Vector3 RecoilTargetRotVelocity;
    private Vector3 HitPoint;
    private Vector3 CameraShakeVec;
    private float AimVelocity = 0;
    private bool IsReloading;
    private float Cam_Recoil;
    private float Cam_RecoilY;
    private float TargetCam_RecoilY;
    private float CurrentAmmo = 0f;
    private scr_GunSO _GunSO;
    #endregion


    #region  - Shoot -
    public override void Execute()
    {
        if (IsReloading)
        {
            return;
        }
        Shoot();
    }
    void CalculateShooting()
    {
        if (CurrentAmmo == 0)
        {
            StartCoroutine(Reload());
            return;
        }
        CalculateBulletTrajectory();
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < BulletWithPath.Count; i++)
        {
            for (int j = 0; j < BulletWithPath[i].path.Length; j++)
            {
                Gizmos.DrawWireSphere(BulletWithPath[i].path[j], 1);
                if (j != BulletWithPath[i].path.Length - 1)
                    Debug.DrawLine(BulletWithPath[i].path[j], BulletWithPath[i].path[j + 1], Color.black, 2);
            }
        }
    }
    void CalculateBulletTrajectory()
    {
        BulletWithPath.RemoveAll(b => b.bullets == null);
        for (int i = 0; i < BulletWithPath.Count; i++)
        {
            RaycastHit[] hit = new RaycastHit[1];
            var numberofHit = Physics.RaycastNonAlloc(BulletWithPath[i].bullets.position, BulletWithPath[i].bullets.forward, hit);
            if (numberofHit > 0)
            {
                if(hit[0].transform.TryGetComponent(out scr_HeathAndArmour scr_Heath))
                {
                    scr_Heath.OnHit(15);
                    Destroy(BulletWithPath[i].bullets.gameObject);
                }
            }
        }
        for (int i = 0; i < BulletWithPath.Count; i++)
        {
            BulletWithPath[i] = new BulletsWithPath
            {
                bullets = BulletWithPath[i].bullets,
                path = BulletWithPath[i].path,
                PathIndex = BulletWithPath[i].PathIndex
            };
            if (BulletWithPath[i].PathIndex != BulletWithPath[i].path.Length)
            {
                BulletWithPath[i].bullets.position = Vector3.MoveTowards(BulletWithPath[i].bullets.position, BulletWithPath[i].path[BulletWithPath[i].PathIndex], _GunSO.BulletSpeed * Time.deltaTime);
                if (BulletWithPath[i].bullets.position == BulletWithPath[i].path[BulletWithPath[i].PathIndex])
                {
                    BulletWithPath[i] = new BulletsWithPath
                    {
                        bullets = BulletWithPath[i].bullets,
                        path = BulletWithPath[i].path,
                        PathIndex = BulletWithPath[i].PathIndex + 1
                    };
                }
            }
            else
            {
                Destroy(BulletWithPath[i].bullets.gameObject);
            }
        }
    }
    void Shoot()
    {
        scr_AudioManeger.Instance.PlayOneShot(_GunSO.GunShots, BulletSpawn.position);
        CurrentAmmo--;
        OnShoot?.Invoke(this, new OnShootEventArgs { CurrentAmmo = CurrentAmmo });
        RecoilTime = Time.deltaTime;
        var bullet = Instantiate(_GunSO.Bullet.transform);
        bullet.position = BulletSpawn.position;
        bullet.forward = BulletSpawn.forward;
        BulletWithPath.Add(new BulletsWithPath
        {
            bullets = bullet,
            path = BulletPath(_GunSO.Iteration, BulletSpawn.position, transform.forward.normalized, _GunSO.BulletSpeed,_GunSO.WindVector),
            PathIndex = 0
        });
        BulletWithPath.RemoveAll(b => b.bullets == null);
    }
    #endregion

    #region - Awake/Start/Update/FixedUpdate -

    private void Start()
    {
        holder.GetWeaponController().InputManeger.AimingInPressed += AimingInPressed;
        holder.GetWeaponController().InputManeger.AimingInReleased += AimingInReleased;
        holder.GetWeaponController().InputManeger.Reload += ReloadEvent;
        _GunSO = GetScr_WeaponSO() as scr_GunSO;
        CurrentAmmo = _GunSO.MaxAmmo;
    }
    private void OnEnable()
    {
        OnShoot?.Invoke(this,new OnShootEventArgs { CurrentAmmo = CurrentAmmo });
    }
    private void Update()
    {
        CalculateShooting();
        CalculateAimingIn();
        CalculateRecoil();
    }
    private void FixedUpdate()
    {
    }

    #endregion
    #region - Reload -
    void ReloadEvent()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(Reload());
    }
    IEnumerator Reload()
    {
        if (CurrentAmmo == _GunSO.MaxAmmo) yield break;
        OnReload?.Invoke(this, new OnReloadEventArgs { CurrentAmmo = CurrentAmmo, isAiming = IsAiming, delay = _GunSO.ReloadTime });
        IsReloading = true;
        yield return new WaitForSeconds(_GunSO.ReloadTime);
        CurrentAmmo = _GunSO.MaxAmmo;
        OnShoot?.Invoke(this, new OnShootEventArgs { CurrentAmmo = CurrentAmmo });
        IsReloading = false;

    }
    #endregion
    #region - AimingIn -
    void CalculateAimingIn()
    {
        var AimTargetPos = Vector3.zero;
        var targetFOV = 60f;
        float reloadLayerWeight = 0;
        if (IsAiming)
        {
            AimTargetPos = holder.Cam().transform.position - holder.GetWeaponParent().position + (transform.position - SightTarget.position) + (holder.Cam().transform.forward * _GunSO.SightOffset);
            targetFOV = _GunSO.FOV;
            if (IsReloading)
            {
                reloadLayerWeight = 0;
            }
        }
        else
        {
            if (IsReloading)
            {
                reloadLayerWeight = 1;
            }
        }
        float _weight = holder.GetWeaponController().animator.GetLayerWeight(holder.GetWeaponController().animator.GetLayerIndex("Reload"));
        _weight = Mathf.SmoothDamp(_weight, reloadLayerWeight, ref AimVelocity, _GunSO.ADSTime);
        Aim_Reload?.Invoke(this, new Aim_ReloadEventArgs { Weight = _weight });
        holder.VCam().m_Lens.FieldOfView = Mathf.SmoothDamp(holder.VCam().m_Lens.FieldOfView, targetFOV, ref FOVFloatVelocity, _GunSO.ADSTime);
        GunAimPosition = transform.localPosition;
        GunAimPosition = Vector3.SmoothDamp(GunAimPosition, transform.InverseTransformVector(AimTargetPos) + (CurrentAttachments.TryGetValue(AttachmentTypes.Sight, out scr_Attachment_SO _sight) ? (_sight as scr_Sight_SO).SightOffset : Vector3.zero), ref GunAimPositionVelocity, _GunSO.ADSTime);
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
        Cam_Recoil += _GunSO.CamRecoilIncrement * _GunSO.Snapiness;
        Cam_Recoil = Mathf.Clamp01(Cam_Recoil);
        if (Cam_RecoilY == TargetCam_RecoilY)
        {
            TargetCam_RecoilY += MathF.Cos(UnityEngine.Random.insideUnitCircle.x) * UnityEngine.Random.Range(-_GunSO.CamRecoil.y, _GunSO.CamRecoil.y);
        }
        Vector3 RecoilPosition = new Vector3
        {
            y = _GunSO.KickBackY.Evaluate(fraction) * _GunSO.KickBackYMultiplier,
            z = _GunSO.KickBackZ.Evaluate(fraction) * _GunSO.KickBackZMultiplier
        };
        RecoilPos = RecoilPosition;
        Vector3 RecoilRotation = new Vector3
        {
            x = _GunSO.RotationX.Evaluate(fraction) * _GunSO.RotationXStrength,
        };
        RecoilRot = RecoilRotation;
    }
    void CalculateRecoil()
    {
        RecoilRot = Vector3.zero;
        RecoilPos = Vector3.zero;
        Cam_RecoilY = 0;
        if (!holder.GetWeaponController().InputManeger.RightClick)
        {
            Cam_Recoil = Mathf.SmoothStep(Cam_Recoil, 0, _GunSO.CameraReturnSpeed);
        }
        if (RecoilTime > 0)
        {
            float fraction = RecoilTime / _GunSO.RecoilSmoothing;
            RecoilTime += Time.deltaTime;// Try Scale Here in future
            if (RecoilTime > _GunSO.RecoilSmoothing)
            {
                RecoilTime = 0;
                fraction = 0;
            }
            Recoil(fraction);
        }
        Cam_RecoilY = Mathf.SmoothStep(Cam_RecoilY, TargetCam_RecoilY, 1);
        Vector3 TargetCameraRecoil = Vector3.zero;
        TargetCameraRecoil = Vector3.Slerp(TargetCameraRecoil, new Vector3(_GunSO.CamRecoil.x, Cam_RecoilY, 0), Cam_Recoil);
        holder.CamRecoilObj().transform.localRotation = Quaternion.Euler(TargetCameraRecoil);
        Vector3 TargetRecoilPos = Vector3.zero;
        TargetRecoilPos = Vector3.Lerp(TargetRecoilPos, RecoilPos, _GunSO.RecoilSmoothing);
        transform.localPosition += TargetRecoilPos;
        Vector3 TargetRecoilRot = Vector3.zero;
        TargetRecoilRot = Vector3.Slerp(TargetRecoilRot, RecoilRot, _GunSO.RecoilSmoothing);
        transform.localRotation = Quaternion.Euler(TargetRecoilRot);
    }
    #endregion
    public void LoadAttachments(List<scr_Attachment_SO> attachment_SOs)
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