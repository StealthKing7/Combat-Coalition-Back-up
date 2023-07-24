using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class scr_BaseWeapon : MonoBehaviour
{
    [SerializeField] scr_WeaponSO WeaponSO;
    private scr_BaseWeapon WeaponObject;


    public virtual void Shoot(Transform BulletSpawn)
    {
        Debug.Log("Base.Shoot()");
    }
    public scr_WeaponSO GetWeaponSO()
    {
        return WeaponSO;
    }
    void SetArms(Transform[] arms)
    {
        for (int i = 0; i < arms.Length; i++)
        {
            arms[i].localPosition = WeaponSO.ArmIKs[i].Position;
            arms[i].localRotation = Quaternion.Euler(WeaponSO.ArmIKs[i].Rotation);
        }
    }
    public void SetUp(GameObject weaponParent, Animator animator, Transform[] arms, Transform sightTarget)
    {
        SetArms(arms);
        sightTarget.localPosition = WeaponSO.SightPos;
        WeaponObject = Instantiate(WeaponSO.weapon, weaponParent.transform);
        WeaponObject.transform.localPosition = WeaponSO.GunPosition.Position;
        WeaponObject.transform.localRotation = Quaternion.Euler(WeaponSO.GunPosition.Rotation);
        animator.runtimeAnimatorController = WeaponSO.controller;
    }

}
