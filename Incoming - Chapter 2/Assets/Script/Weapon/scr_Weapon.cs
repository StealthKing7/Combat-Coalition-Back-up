using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class scr_Weapon : scr_BaseWeapon
{
    [SerializeField] private scr_WeaponSO scr_WeaponSO;
    public scr_WeaponSO GetWeaponSO()
    {
        return scr_WeaponSO; 
    }
    public void Shoot(Transform BulletSpawn)
    {
        Instantiate(GetWeaponSO().Bullet, BulletSpawn.position, Quaternion.identity);
    }
}
