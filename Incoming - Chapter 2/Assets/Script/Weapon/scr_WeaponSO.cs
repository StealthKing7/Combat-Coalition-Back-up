using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static scr_Models;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapons")]
public class scr_WeaponSO : ScriptableObject
{
    [Header("Gun Properties")]
    public GameObject prefab;
    public RuntimeAnimatorController controller;
    [Header("Sight")]
    public Vector3 SightPos;
    public float SightOffset;
    public float ADSTime;
    [Header("Shooting Properties")]
    public GameObject Bullet;
    public List<WeaponFireType> AllowedFireTypes;
    [Header("Transforms")]
    public MyTransform GunPosition;
    public MyTransform[] ArmIKs;
    public scr_Weapon GetWeapon()
    {
        return prefab.GetComponent<scr_Weapon>();
    }

}
