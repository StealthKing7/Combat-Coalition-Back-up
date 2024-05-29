using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static scr_Models;
public class scr_GunAttachmentsSystem : MonoBehaviour
{
    public static scr_GunAttachmentsSystem Instance { get; private set; }
    public event EventHandler OnAnyPartChanged;
    [field : SerializeField] public List<scr_WeaponSO> WeaponSOList { get; private set; }
    [SerializeField] private scr_WeaponSO CurrentWeaponSO;
    private List<scr_WeaponSO> AllWeaponsSelected = new List<scr_WeaponSO>();
    private scr_GunAttachmentPreview WeaponPreview;



    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetWeaponBody(WeaponSOList[0]);
    }
    public void SetWeaponBody(scr_WeaponSO WeaponSO)
    {
        Vector3 previousEulerAngles = Vector3.zero;

        if (WeaponPreview != null)
        {
            // Clear previous WeaponComplete
            previousEulerAngles = WeaponPreview.transform.eulerAngles;
            Destroy(WeaponPreview.gameObject);
        }

        CurrentWeaponSO = WeaponSO;

        Transform weaponBodyTransform = Instantiate(WeaponSO.WeaponAttachmentModel);
        weaponBodyTransform.eulerAngles = previousEulerAngles;
        WeaponPreview = weaponBodyTransform.GetComponent<scr_GunAttachmentPreview>();
        if (WeaponSO.WeaponType == WeaponType.Melee) return;
        Instantiate(WeaponPreview.PrefabUI, weaponBodyTransform);
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            AllWeaponsSelected.Add(WeaponPreview.GetWeaponBodySO());
        }
    }
    public int GetPartIndex(AttachmentTypes partType)
    {
        scr_Attachment_SO equippedWeaponPartSO = WeaponPreview.GetWeaponPartSO(partType);
        if (equippedWeaponPartSO == null || CurrentWeaponSO.WeaponType == WeaponType.Melee)
        {
            return 0;
        }
        else
        {
            var gun_SO = CurrentWeaponSO as scr_GunSO;
            List<scr_Attachment_SO> weaponPartSOList = gun_SO.ValidAttachments.GetWeaponPartSOList(partType);
            int partIndex = weaponPartSOList.IndexOf(equippedWeaponPartSO);
            return partIndex;
        }
    }

    public void ChangePart(AttachmentTypes partType)
    {
        scr_Attachment_SO equippedWeaponPartSO = WeaponPreview.GetWeaponPartSO(partType);
        var gun_SO = CurrentWeaponSO as scr_GunSO;
        if (equippedWeaponPartSO == null)
        {
            WeaponPreview.SetAttachments(gun_SO.ValidAttachments.GetWeaponPartSOList(partType)[0]);
            OnAnyPartChanged?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            List<scr_Attachment_SO> weaponPartSOList = gun_SO.ValidAttachments.GetWeaponPartSOList(partType);
            int partIndex = weaponPartSOList.IndexOf(equippedWeaponPartSO);
            int nextPartIndex = (partIndex + 1) % weaponPartSOList.Count;
            WeaponPreview.SetAttachments(weaponPartSOList[nextPartIndex]);
            OnAnyPartChanged?.Invoke(this, EventArgs.Empty);
        }

    }

    public void RandomizeParts()
    {
        foreach (AttachmentTypes partType in WeaponPreview.GetWeaponPartTypeList())
        {
            int randomAmount = UnityEngine.Random.Range(0, 50);
            for (int i = 0; i < randomAmount; i++)
            {
                ChangePart(partType);
            }
        }
    }

    public List<scr_WeaponSO> GetWeaponBodySO()
    {
        return AllWeaponsSelected;
    }

    public void ChangeWeapon(scr_WeaponSO weapon)
    {
        var weapontype = weapon.WeaponType;
        List<scr_WeaponSO> WeaponsOfSameType = new List<scr_WeaponSO>();
        foreach (var _weapon in WeaponSOList)
        {
            if (_weapon.WeaponType == weapontype)
            {
                WeaponsOfSameType.Add(_weapon);
            }
        }
        int Index = WeaponsOfSameType.IndexOf(CurrentWeaponSO);
        int nextIndex = (Index + 1) % WeaponsOfSameType.Count;
        if (nextIndex == Index)
        {
            nextIndex = 0;
        }
        SetWeaponBody(WeaponsOfSameType[nextIndex]);
    }

    public scr_GunAttachmentPreview GetWeaponComplete()
    {
        return WeaponPreview;
    }

    public void SetWeaponComplete(scr_GunAttachmentPreview weaponComplete)
    {
        if (WeaponPreview != null)
        {
            // Clear previous WeaponComplete
            Destroy(WeaponPreview.gameObject);
        }

        CurrentWeaponSO = weaponComplete.GetWeaponBodySO();

        WeaponPreview = weaponComplete;
    }

    public void ResetWeaponRotation()
    {
        WeaponPreview.transform.eulerAngles = Vector3.zero;
    }
}
