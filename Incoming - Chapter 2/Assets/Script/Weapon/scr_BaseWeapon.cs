using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class scr_BaseWeapon : MonoBehaviour
{
    private scr_Weapon weapon;
    private GameObject WeaponObject;

    public void ClearWeapon()
    {
        weapon = null;
        Destroy(WeaponObject);
    }
    public void SetWeapon(scr_Weapon _Weapon)
    {
        weapon = _Weapon;
    }
    public scr_Weapon GetWeapon()
    {
        return weapon;
    }
    void SetArms(Transform[] arms)
    {
        for (int i = 0; i < arms.Length; i++)
        {
            arms[i].localPosition = weapon.GetWeaponSO().ArmIKs[i].Position;
            arms[i].localRotation = Quaternion.Euler(weapon.GetWeaponSO().ArmIKs[i].Rotation);
        }
    }
    public void SetUp(GameObject weaponParent, Animator animator, Transform[] arms, Transform sightTarget)
    {
        SetArms(arms);
        sightTarget.localPosition = weapon.GetWeaponSO().SightPos;
        WeaponObject = Instantiate(weapon.GetWeaponSO().prefab, weaponParent.transform);
        WeaponObject.transform.localPosition = weapon.GetWeaponSO().GunPosition.Position;
        WeaponObject.transform.localRotation = Quaternion.Euler(weapon.GetWeaponSO().GunPosition.Rotation);
        animator.runtimeAnimatorController = weapon.GetWeaponSO().controller;
    }

}
