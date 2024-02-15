using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_WeaponHud : MonoBehaviour
{
    [SerializeField] private Image WeaponIcon;
    [SerializeField] private scr_GunHud scr_GunHud;
    private void Start()
    {
        scr_GameManeger.Instance.GetPlayerList().ForEach(p => p.WeaponController.OnWeaponEquiped += OnWeaponEquiped);
    }

    private void OnWeaponEquiped(object sender, scr_WeaponController.OnWeaponEquipedEventArgs e)
    {
        WeaponIcon.sprite = e.weapon.GetScr_WeaponSO().Icon;
        scr_GunHud.gameObject.SetActive(e.weapon.GetScr_WeaponSO().WeaponType == scr_Models.WeaponType.Gun);
    }
}