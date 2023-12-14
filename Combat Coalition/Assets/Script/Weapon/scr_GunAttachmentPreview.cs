using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static scr_Models;

public class scr_GunAttachmentPreview : MonoBehaviour
{
    private Dictionary<AttachmentTypes, AttachedWeaponPart> CurrentAttachments = new Dictionary<AttachmentTypes, AttachedWeaponPart>();
    [SerializeField] scr_WeaponSO WeaponSO;
    [SerializeField] AttachmentsPoints[] AttachmentsPoints;
    public Transform PrefabUI;
    private void Start()
    {
        foreach (AttachmentsPoints attachmentsPoint in AttachmentsPoints)
        {
            CurrentAttachments[attachmentsPoint.AttachmentType] = new AttachedWeaponPart
            {
                AttachmentTypePoint = attachmentsPoint,
            };
        }
        if (WeaponSO.WeaponType == WeaponType.Melee) return;
        var gunSO = WeaponSO as scr_GunSO;
        foreach (var part in gunSO.DefaultAttachments)
        {
            SetAttachments(part);
        }
    }

    public void SetAttachments(scr_Attachment_SO AttachmentSO)
    {
        // Destroy currently attached part
        if (CurrentAttachments[AttachmentSO.AttachmentType].spawnedTransform != null)
        {
            Destroy(CurrentAttachments[AttachmentSO.AttachmentType].spawnedTransform.gameObject);
        }
        // Spawn new part
        Transform spawnedPartTransform = Instantiate(AttachmentSO.Model);
        AttachedWeaponPart attachedWeaponPart = CurrentAttachments[AttachmentSO.AttachmentType];
        attachedWeaponPart.spawnedTransform = spawnedPartTransform;
        Transform attachPointTransform = attachedWeaponPart.AttachmentTypePoint.Point;
        spawnedPartTransform.parent = attachPointTransform;
        spawnedPartTransform.localEulerAngles = Vector3.zero;
        spawnedPartTransform.localPosition = Vector3.zero;
        attachedWeaponPart.weaponPartSO = AttachmentSO;
        CurrentAttachments[AttachmentSO.AttachmentType] = attachedWeaponPart;
    }
    public scr_Attachment_SO GetWeaponPartSO(AttachmentTypes partType)
    {
        AttachedWeaponPart attachedWeaponPart = CurrentAttachments[partType];
        return attachedWeaponPart.weaponPartSO;
    }

    public List<AttachmentTypes> GetWeaponPartTypeList()
    {
        return new List<AttachmentTypes>(CurrentAttachments.Keys);
    }

    public scr_WeaponSO GetWeaponBodySO()
    {
        return WeaponSO;
    }
    public List<scr_Attachment_SO> Save()
    {
        List<scr_Attachment_SO> weaponPartSOList = new List<scr_Attachment_SO>();
        foreach (AttachmentTypes partType in CurrentAttachments.Keys)
        {
            if (CurrentAttachments[partType].weaponPartSO != null)
            {
                weaponPartSOList.Add(CurrentAttachments[partType].weaponPartSO);
            }
        }
        return weaponPartSOList;
    }
}
