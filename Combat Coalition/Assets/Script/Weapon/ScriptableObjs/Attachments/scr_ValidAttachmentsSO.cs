using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class scr_ValidAttachmentsSO : ScriptableObject
{

    public List<scr_Attachment_SO> weaponPartSOList;


    public List<scr_Attachment_SO> GetWeaponPartSOList(scr_Models.AttachmentTypes partType)
    {
        List<scr_Attachment_SO> returnWeaponPartSOList = new List<scr_Attachment_SO>();

        foreach (scr_Attachment_SO weaponPartSO in weaponPartSOList)
        {
            if (weaponPartSO.AttachmentType == partType)
            {
                returnWeaponPartSOList.Add(weaponPartSO);
            }
        }

        return returnWeaponPartSOList;
    }

}
