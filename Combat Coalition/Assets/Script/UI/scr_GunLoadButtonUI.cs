using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class scr_GunLoadButtonUI : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;


    private string weaponCompleteJson;


    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(LoadWeapon);
    }

    private void LoadWeapon()
    {
        //scr_GunAttachmentPreview weaponComplete = scr_GunAttachmentPreview.LoadSpawnWeaponComplete(weaponCompleteJson, true);
        //scr_GunAttachmentsSystem.Instance.SetWeaponComplete(weaponComplete);
    }


    public void Setup(string weaponCompleteJson, Texture2D texture2D)
    {
        this.weaponCompleteJson = weaponCompleteJson;
        rawImage.texture = texture2D;
    }

}
