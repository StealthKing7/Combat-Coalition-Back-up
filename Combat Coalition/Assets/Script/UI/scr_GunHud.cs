using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class scr_GunHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI CurrentAmmo;
    [SerializeField] Image SingleFire; 
    [SerializeField] Image AutoFire; 
    private void Start()
    {
        scr_GameManeger.Instance.GetPlayerList().ForEach(p =>
        {
            p.WeaponController.OnWeaponEquiped += WeaponController_OnWeaponEquiped;
        });
    }

    private void WeaponController_OnWeaponEquiped(scr_WeaponController sender, scr_WeaponController.OnWeaponEquipedEventArgs e)
    {
        if (e.weapon.GetScr_WeaponSO().WeaponType == scr_Models.WeaponType.Gun)
            (e.weapon as scr_Gun).OnShoot += Scr_GunHud_OnShoot;
            UpdateFireTypeUI(sender);
    }
    void UpdateFireTypeUI(scr_WeaponController WeaponController)
    {
        WeaponController.OnFireTypeChange += WeaponController_OnFireTypeChange;
    }
    private void WeaponController_OnFireTypeChange(object sender, scr_WeaponController.OnFireTypeChangeEventArgs e)
    {
        switch (e.FireType)
        {
            case scr_Models.GunFireType.SemiAuto:
                AutoFire.gameObject.SetActive(false);
                SingleFire.gameObject.SetActive(true);
                break;
            default:
                AutoFire.gameObject.SetActive(true);
                SingleFire.gameObject.SetActive(false);
                break;
        }
    }

    private void Scr_GunHud_OnShoot(object sender, scr_Gun.OnShootEventArgs e)
    {
        CurrentAmmo.text = e.CurrentAmmo.ToString();
        CurrentAmmo.text += "/";
    }

}
