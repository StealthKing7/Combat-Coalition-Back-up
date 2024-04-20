using System.Collections.Generic;
using static scr_Models;
using UnityEngine.Animations;
using System.Linq;
using UnityEngine;
using System;
using System.Collections;

public class scr_WeaponController : MonoBehaviour,scr_WeaponHolder
{
    #region - Parameters -
    public event EventHandler<OnWeaponEquipedEventArgs> OnWeaponEquiped;
    public class OnWeaponEquipedEventArgs : EventArgs 
    {
        public Animator controller;
        public scr_BaseWeapon weapon;
        public List<scr_Attachment_SO> attachment_SO;
    }

    public event EventHandler<OnFireTypeChangeEventArgs> OnFireTypeChange;
    public class OnFireTypeChangeEventArgs : EventArgs { public WeaponFireType FireType; }
    private scr_BaseWeapon CurrentWeapon;
    private scr_Pickable InRangeWeapon;
    public scr_InputManeger InputManeger {  get; private set; }
    private RaycastHit[] hitInfo = new RaycastHit[1];
    private scr_CharacterController CharacterController;
    [SerializeField] LayerMask RetractLayerMask;
    [SerializeField] LayerMask WeaponLayerMask;
    [field: SerializeField] public LayerMask BulletIgnoreLayer { get; private set; }
    public scr_WeaponSO weaponSO { get; private set; }
    private scr_GunSO GunSO;
    private scr_MeleeSO MeleeSO;
    [Header("References")]
    [SerializeField] Transform WeaponAimPiviot;
    [SerializeField] Transform WeaponParent;
    [SerializeField] Animator animator;
    [field: SerializeField] public Transform SwayObj { get;  set; }
    //Setting
    [field: SerializeField] public WeaponSettingsModel Settings { get; private set; }
    //Breathing
    [field: SerializeField] public float SwayAmountA { get; private set; } = 1;
    [field: SerializeField] public float SwayAmountB { get; private set; } = 2;
    [field: SerializeField] public float SwayScale { get; private set; } = 600;
    [field: SerializeField] public float SwayLerpSpeed { get; private set; } = 14;
    [Header("Shooting")]
    [SerializeField] WeaponFireType currentFireType;
    private Vector3 LastWeaponPos;
    private float HoldTimeBegin;
    private float nextTimetoFire = 0f;
    #endregion

    #region - Awake/Start/Update/LateUpdate -

    private void Awake()
    {
        weaponSO = scr_GameManeger.Instance._WeaponSO;
        if (weaponSO.WeaponType == WeaponType.Gun)
        {
            GunSO = weaponSO as scr_GunSO;
        }
        else
        {
            MeleeSO = weaponSO as scr_MeleeSO;
        }
        SetWeapon(scr_BaseWeapon.SpawnWeapon(weaponSO, this));
    }
    private void Start()
    {
        InputManeger = CharacterController.InputManeger;
        InputManeger.FireType += ChangeFireType;
        animator.runtimeAnimatorController = weaponSO.controller;
        if (weaponSO.WeaponType == WeaponType.Gun)
            currentFireType = GunSO.AllowedFireTypes.First();
        StartCoroutine(Delay());
    }
    private void Update()
    {
        CalculateAttack();
        CalculateWeaponAimTarget();
        CalculateEqupingWeapon();
    }
    #endregion

    #region - Attack -

    void CalculateAttack()
    {
        if (!InputManeger.RightClick) return;
        switch (weaponSO.WeaponType)
        {
            case WeaponType.Gun:
                switch (currentFireType)
                {
                    case WeaponFireType.FullyAuto:
                        if (Time.time >= nextTimetoFire)
                        {
                            nextTimetoFire = Time.time + 1 / GunSO.FireRate;
                            CurrentWeapon.Execute();
                        }
                        break;
                    case WeaponFireType.SemiAuto:
                        CurrentWeapon.Execute();
                        InputManeger.RightClick = false;
                        break;
                }
                break;
            case WeaponType.Melee:
                CurrentWeapon.Execute();
                InputManeger.RightClick = false;
                break;
        }
    }

    #endregion    

    #region - Rotation - 

    void CalculateWeaponAimTarget()
    {
        if (!Physics.Raycast(CharacterController.MainCamera.transform.position, CharacterController.MainCamera.transform.forward, 1f, RetractLayerMask))
        {
            WeaponAimPiviot.localRotation = Quaternion.Slerp(WeaponAimPiviot.localRotation, Quaternion.identity, Settings.RetractSmoothing);
        }
        else
        {
            WeaponAimPiviot.localRotation = Quaternion.Slerp(WeaponAimPiviot.localRotation, Quaternion.Euler(Settings.RetractAngle), Settings.RetractSmoothing);
        }
    }
    #endregion

    void ChangeFireType()
    {
        int index = GunSO.AllowedFireTypes.IndexOf(currentFireType);
        if (weaponSO.WeaponType == WeaponType.Gun)
            currentFireType = GunSO.AllowedFireTypes[(index + 1) % GunSO.AllowedFireTypes.Count];
        OnFireTypeChange?.Invoke(this, new OnFireTypeChangeEventArgs { FireType = currentFireType });
    }

    #region  - Equiping Weapon -
    IEnumerator Delay()
    {
        yield return null;
        OnWeaponEquiped?.Invoke(this, new OnWeaponEquipedEventArgs
        {
            weapon = CurrentWeapon,
            controller = animator,
            attachment_SO = scr_GameManeger.Instance.GetAttachments()
        });
    }
    void CalculateEqupingWeapon()
    {
        Ray camRay = Cam().ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        var RaycastInt = Physics.RaycastNonAlloc(camRay, hitInfo, Settings.WeaponSearchRaduis, WeaponLayerMask, QueryTriggerInteraction.Ignore);
        float holdTime = 0;
        if (RaycastInt == 0)
        {
            InRangeWeapon = null;
            scr_UI_Maneger.Instance.Interact(InRangeWeapon, holdTime);
            return;
        }
        InRangeWeapon = hitInfo[0].collider.gameObject.GetComponent<scr_Pickable>();
        if (!InputManeger.IsInteractPressed)
        {
            HoldTimeBegin = Time.time;
        }
        holdTime = Time.time - HoldTimeBegin;
        holdTime = Mathf.Clamp01(holdTime);
        if (holdTime >= 1)
        {
            Equip();
            InputManeger.IsInteractPressed = false;
        }
        scr_UI_Maneger.Instance.Interact(InRangeWeapon,holdTime);
    }
    void Equip()
    {
        if (HasWeapon())
        {
            LastWeaponPos = InRangeWeapon.transform.position;
            SetWeapon(scr_BaseWeapon.SpawnWeapon(InRangeWeapon.Weapon.GetScr_WeaponSO(), this));
            OnWeaponEquiped?.Invoke(this, new OnWeaponEquipedEventArgs
            {
                controller = animator,
                weapon = CurrentWeapon,
                attachment_SO = InRangeWeapon.attachment_SOs
            });
            weaponSO = CurrentWeapon.GetScr_WeaponSO();
            InRangeWeapon.DestroySelf();
            Cam().fieldOfView = 60f;
        }
    }
    #endregion

    public void SetCharcterController(scr_CharacterController characterController)
    {
        CharacterController = characterController;
    }

    #region - Holder -
    public bool HasWeapon()
    {
        return CurrentWeapon != null;
    }
    public void SetWeapon(scr_BaseWeapon _weapon)
    {
        CurrentWeapon = _weapon;
    }
    public scr_BaseWeapon GetWeapon()
    {
        return CurrentWeapon;
    }
    public Transform GetWeaponParent()
    {
        return WeaponParent;
    }
    public void DropWeapon()
    {
        if (weaponSO.WeaponPickable != null)
        {
            scr_BaseWeapon.SpawnWeaponInWorld(weaponSO, LastWeaponPos);
        }
        CurrentWeapon.DestroySelf();
    }
    public Camera Cam()
    {
        return CharacterController.MainCamera;
    }
    public Transform CamHolder()
    {
        return CharacterController.CameraHolder;
    }
    public scr_WeaponController GetWeaponController()
    {
        return this;
    }
    #endregion
}
