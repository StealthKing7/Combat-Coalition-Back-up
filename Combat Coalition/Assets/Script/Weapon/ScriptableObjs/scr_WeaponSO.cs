using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static scr_Models;
public class scr_WeaponSO : ScriptableObject
{
    

    public string WeaponName;
    public Vector3 WeaponPos;
    public Sprite Icon;
    [field:SerializeField] public scr_BaseWeapon Weapon { get; private set; }
    [field: SerializeField] public scr_Pickable WeaponPickable { get; private set; }
    [field:SerializeField] public Transform WeaponAttachmentModel {  get; private set; }

    [Header("Gun Properties")]
    public RuntimeAnimatorController controller;
    public bool Is2Handed;
    public WeaponType WeaponType;
    public float EquipingTime = 1;
}