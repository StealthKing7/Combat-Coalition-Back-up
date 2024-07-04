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
    public delegate void OnEquippedEvent(scr_WeaponController sender, OnWeaponEquipedEventArgs e);
    public event OnEquippedEvent OnWeaponEquiped;
    public class OnWeaponEquipedEventArgs : EventArgs 
    {
        public Animator controller;
        public scr_BaseWeapon weapon;
    }
    public event EventHandler<OnFireTypeChangeEventArgs> OnFireTypeChange;
    public class OnFireTypeChangeEventArgs : EventArgs { public WeaponFireType FireType; }
    private scr_BaseWeapon CurrentWeapon;
    private List<scr_BaseWeapon> TotalWeapons = new List<scr_BaseWeapon>();
    private scr_Pickable InRangeWeapon;
    public scr_InputManeger InputManeger {  get; private set; }
    private RaycastHit[] hitInfo = new RaycastHit[1];
    private scr_CharacterController CharacterController;
    [SerializeField] LayerMask RetractLayerMask;
    [SerializeField] LayerMask WeaponLayerMask;
    [field: SerializeField] public LayerMask BulletIgnoreLayer { get; private set; }
    public scr_WeaponSO CurrentWeaponSO { get; private set; }
    public List<scr_WeaponSO> TotalWeaponSO;
    private scr_GunSO GunSO;
    private scr_MeleeSO MeleeSO;
    [Header("References")]
    [SerializeField] Transform WeaponAimPiviot;
    [SerializeField] Transform WeaponParent;
    public Animator animator;
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
    private int CurrentWeaponSOIndex = 0;
    #endregion

    #region - Awake/Start/Update/LateUpdate -

    private void Awake()
    {
        scr_GameManeger.Instance.AllWeaponSO.ForEach(i => TotalWeaponSO.Add(i.scr_WeaponSO));
        foreach (var weaponso in TotalWeaponSO)
        {
            TotalWeapons.Add(scr_BaseWeapon.SpawnWeapon(weaponso, this));
        }
        GunSO = (TotalWeaponSO[CurrentWeaponSOIndex].WeaponType == WeaponType.Gun) ? (CurrentWeaponSO = TotalWeaponSO[CurrentWeaponSOIndex]) as scr_GunSO : null;
        MeleeSO = (TotalWeaponSO[CurrentWeaponSOIndex].WeaponType == WeaponType.Melee) ? (CurrentWeaponSO = TotalWeaponSO[CurrentWeaponSOIndex]) as scr_MeleeSO : null;
        transform.localPosition = CurrentWeaponSO.WeaponPos;
    }
    private void Start()
    {
        InputManeger = CharacterController.InputManeger;
        InputManeger.FireType += ChangeFireType;
        InputManeger.ScrollUp += ScrollUp;
        InputManeger.ScrollDown += ScrollDown;
        animator.runtimeAnimatorController = CurrentWeaponSO.controller;
        if (CurrentWeaponSO.WeaponType == WeaponType.Gun)
            currentFireType = GunSO.AllowedFireTypes.First();
        StartCoroutine(Delay());
        InputManeger.RightClickPressed += () => { if (CurrentWeaponSO.WeaponType == WeaponType.Melee) return; scr_AudioManeger.Instance.PlayOneShot(GunSO.TriggerPressed, CurrentWeapon.transform.position); };
        TotalWeapons.ForEach(weapon =>
        {
            if (TotalWeapons.IndexOf(weapon) == CurrentWeaponSOIndex)
            {
                SetWeapon(weapon);
                return;
            }
            weapon.gameObject.SetActive(false);
        });
    }
    private void Update()
    {
        CalculateAttack();
        //CalculateWeaponAimTarget();
        CalculateEqupingWeapon();
    }
    #endregion

    #region - Attack -

    void CalculateAttack()
    {
        if (!InputManeger.RightClick) return;
        switch (CurrentWeaponSO.WeaponType)
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
        if (CurrentWeaponSO.WeaponType == WeaponType.Gun)
            currentFireType = GunSO.AllowedFireTypes[(index + 1) % GunSO.AllowedFireTypes.Count];
        OnFireTypeChange?.Invoke(this, new OnFireTypeChangeEventArgs { FireType = currentFireType });
    }

    #region - WeaponSwitching -
    void ScrollUp()
    {
        if (TotalWeapons.Count < 2) return;
        int index = TotalWeapons.IndexOf(CurrentWeapon);
        SetWeapon(TotalWeapons[(index + 1) % TotalWeapons.Count]);
        Debug.Log(index + "," + CurrentWeapon.name);
        SetUpWeapon();
    }
    void ScrollDown()
    {
        if (TotalWeapons.Count < 2) return;
        int index = TotalWeapons.IndexOf(CurrentWeapon);
        if (index == 0) index = TotalWeapons.Count;
        SetWeapon(TotalWeapons[index - 1]);
        SetUpWeapon();
    }
    #endregion
    #region  - Equiping Weapon -
    void SetUpWeapon()
    {
        CurrentWeaponSOIndex = TotalWeapons.IndexOf(CurrentWeapon);
        CurrentWeaponSO = TotalWeaponSO[CurrentWeaponSOIndex];
        TotalWeapons.ForEach(weapon =>
        {
            if (TotalWeapons.IndexOf(weapon) != CurrentWeaponSOIndex)
            {
                weapon.gameObject.SetActive(false);
                return;
            }
            weapon.gameObject.SetActive(true);
        });
        GunSO = (TotalWeaponSO[CurrentWeaponSOIndex].WeaponType == WeaponType.Gun) ? (CurrentWeaponSO = TotalWeaponSO[CurrentWeaponSOIndex]) as scr_GunSO : null;
        MeleeSO = (TotalWeaponSO[CurrentWeaponSOIndex].WeaponType == WeaponType.Melee) ? (CurrentWeaponSO = TotalWeaponSO[CurrentWeaponSOIndex]) as scr_MeleeSO : null;
        if (CurrentWeaponSO.WeaponType == WeaponType.Gun)
        {
            currentFireType = GunSO.AllowedFireTypes.First();
        }
        OnWeaponEquiped?.Invoke(this, new OnWeaponEquipedEventArgs
        {
            controller = animator,
            weapon = CurrentWeapon,
        });
        transform.localPosition = CurrentWeaponSO.WeaponPos;
    }
    IEnumerator Delay()
    {
        yield return null;
        OnWeaponEquiped?.Invoke(this, new OnWeaponEquipedEventArgs
        {
            weapon = CurrentWeapon,
            controller = animator
        });
        for (int i = 0; i < scr_GameManeger.Instance.AllWeaponSO.Count; i++)
        {
            TotalWeaponSO.ForEach(gun =>
            {
                if (gun.WeaponType == WeaponType.Melee) return;
                if (scr_GameManeger.Instance.AllWeaponSO[i].scr_WeaponSO == gun)
                {
                    var targetweapon = TotalWeapons.Find(w2 => w2.GetScr_WeaponSO() == gun);
                    (targetweapon as scr_Gun).LoadAttachments(scr_GameManeger.Instance.AllWeaponSO[i].AttachmentSOList);
                }
            });
        }
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
            });
            CurrentWeaponSO = CurrentWeapon.GetScr_WeaponSO();
            InRangeWeapon.DestroySelf();
            Cam().fieldOfView = 60f;
        }
        transform.localPosition = CurrentWeaponSO.WeaponPos;
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
        if (CurrentWeaponSO.WeaponPickable != null)
        {
            scr_BaseWeapon.SpawnWeaponInWorld(CurrentWeaponSO, LastWeaponPos);
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
