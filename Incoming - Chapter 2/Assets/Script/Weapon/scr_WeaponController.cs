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

    public Transform debugobj;
    public event EventHandler<OnWeaponEquipedEventArgs> OnWeaponEquiped;
    public class OnWeaponEquipedEventArgs : EventArgs 
    {
        public Animator controller;
        public scr_BaseWeapon weapon;
        public float _SwayAmountA;
        public float _SwayAmountB;
        public float _SwayScale;
        public float _SwayLerpSpeed;
        public Transform _SwayObj;
    }
    private scr_BaseWeapon CurrentWeapon;
    private scr_Pickable InRangeWeapon;
    private scr_InputManeger inputManeger;
    private RaycastHit[] hitInfo = new RaycastHit[1];
    private scr_CharacterController CharacterController;
    [SerializeField] LayerMask RetractLayerMask;
    [SerializeField] LayerMask WeaponLayerMask;
    [field: SerializeField] public LayerMask BulletIgnoreLayer { get; private set; }
    [field:SerializeField] public scr_WeaponSO weaponSO { get; private set; }
    [Header("References")]
    [SerializeField] Transform WeaponAimPiviot;
    [SerializeField] Transform WeaponParent;
    [SerializeField] Animator animator;
    [SerializeField] Transform SwayObj;
    //Setting
    [field: SerializeField] public WeaponSettingsModel Settings { get; private set; }
    //Breathing
    [Header("Weapon Sway")]
    [SerializeField] float SwayAmountA = 1;
    [SerializeField] float SwayAmountB = 2;
    [SerializeField] float SwayScale = 600;
    [SerializeField] float SwayLerpSpeed = 14;
    [Header("Shooting")]
    [SerializeField] float RateOfFire;
    [SerializeField] WeaponFireType currentFireType;
    private Vector3 LastWeaponPos;
    #endregion

    #region - Awake/Start/Update/LateUpdate -

    private void Awake()
    {
        SetWeapon(scr_BaseWeapon.SpawnWeapon(weaponSO, this));
        
        CurrentWeapon.SetUp(SwayAmountA, SwayAmountB, SwayLerpSpeed, SwayObj, SwayScale);
    }
    private void Start()
    {
        inputManeger = scr_InputManeger.Instance;
        inputManeger.Interact += Equip;
        animator.runtimeAnimatorController = weaponSO.controller;
        StartCoroutine(EquipCoroutine());
        //currentFireType = CurrentWeapon.GetScr_WeaponSO().AllowedFireTypes.First();
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
        if (inputManeger.IsShooting && CurrentWeapon != null)
        {
            CurrentWeapon.Execute();
            if (currentFireType == WeaponFireType.SemiAuto)
            {
                inputManeger.IsShooting = false;
            }
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


    #region  - Equiping Weapon -
    IEnumerator EquipCoroutine()
    {
        yield return null;
        OnWeaponEquiped?.Invoke(this, new OnWeaponEquipedEventArgs { weapon = CurrentWeapon, controller = animator });
    }
    void CalculateEqupingWeapon()
    {
        Ray camRay = Cam().ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        var RaycastInt = Physics.RaycastNonAlloc(camRay, hitInfo, Settings.WeaponSearchRaduis, WeaponLayerMask, QueryTriggerInteraction.Ignore);
        if (RaycastInt > 0)
        {
            InRangeWeapon = hitInfo[0].collider.gameObject.GetComponent<scr_Pickable>();
        }
        else
        {
            InRangeWeapon = null;
        }
    }
    void Equip()
    {
        if (InRangeWeapon == null) return;
        if (HasWeapon() && InRangeWeapon.Weapon.GetScr_WeaponSO() != CurrentWeapon.GetScr_WeaponSO())
        {
            LastWeaponPos = InRangeWeapon.transform.position;
            SetWeapon(scr_BaseWeapon.SpawnWeapon(InRangeWeapon.Weapon.GetScr_WeaponSO(), this));
            OnWeaponEquiped?.Invoke(this, new OnWeaponEquipedEventArgs
            {
                controller = animator,
                weapon = CurrentWeapon,
                _SwayAmountA = SwayAmountA,
                _SwayAmountB = SwayAmountB,
                _SwayLerpSpeed = SwayLerpSpeed,
                _SwayObj = SwayObj,
                _SwayScale = SwayScale,
            });
            CurrentWeapon.SetUp(SwayAmountA, SwayAmountB, SwayLerpSpeed, SwayObj, SwayScale);
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
        scr_BaseWeapon.SpawnWeaponInWorld(weaponSO, LastWeaponPos);
        CurrentWeapon.DestroySelf();
    }
    public Camera Cam()
    {
        return CharacterController.MainCamera;
    }
    public scr_WeaponController GetWeaponController()
    {
        return this;
    }
    #endregion
}
