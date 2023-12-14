using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Pickable : MonoBehaviour
{
    [field:SerializeField]public List<scr_Attachment_SO> attachment_SOs {  get; private set; }
    [field : SerializeField] public scr_BaseWeapon Weapon { get; private set; }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    public void  SetAttachments(List<scr_Attachment_SO> scr_Attachment_SOs)
    {
        attachment_SOs = scr_Attachment_SOs;
    }
}
