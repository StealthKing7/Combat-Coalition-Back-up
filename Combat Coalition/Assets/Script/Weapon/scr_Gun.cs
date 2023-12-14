using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static scr_Models;

public class scr_Gun : scr_BaseWeapon
{
    #region - Parameters - 
    private Ray ray;
    private Dictionary<AttachmentTypes, scr_Attachment_SO> CurrentAttachments = new Dictionary<AttachmentTypes, scr_Attachment_SO>();
    private List<scr_Bullet> bullets = new List<scr_Bullet>();
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
    private Vector3 BulletTargetPos;
    private Vector3 GunWorldPosition;
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
        foreach (var bullet in bullets)
        {
            if((BulletTargetPos - bullet.transform.position).magnitude <= 0)
            {
                Debug.Log("Hit");
            }
        }
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

    #region - Awake/Start/Update/FixedUpdate -

    private void Start()
    {
        GunWorldPosition = transform.position;
        scr_InputManeger.Instance.AimingInPressed += AimingInPressed;
        scr_InputManeger.Instance.AimingInReleased += AimingInReleased;
        holder.GetWeaponController().OnWeaponEquiped += Scr_Gun_OnWeaponEquiped;
        _GunSO = GetScr_WeaponSO() as scr_GunSO;
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
    }
    void CalculateRecoil()
    {
        RecoilTargetPos = Vector3.zero;
        RecoilTargetRot = Vector3.zero;
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
        //holder.Cam().transform.localRotation = Quaternion.Euler(Vector3.SmoothDamp(holder.Cam().transform.localRotation.eulerAngles, RecoilTargetRot, ref RecoilTargetRotVelocity, RecoilTime));
        transform.localPosition += RecoilPos;
        transform.localRotation = Quaternion.Euler(Recoilrotation);
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
