using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static scr_Models;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapons")]
public class scr_WeaponSO : ScriptableObject
{
    [field:SerializeField]public scr_BaseWeapon weapon { get; private set; }
    [Header("Gun Properties")]
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

}
