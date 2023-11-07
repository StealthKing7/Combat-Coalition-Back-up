using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Pickable : MonoBehaviour
{
    [field : SerializeField] public scr_BaseWeapon Weapon { get; private set; }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
        
}
