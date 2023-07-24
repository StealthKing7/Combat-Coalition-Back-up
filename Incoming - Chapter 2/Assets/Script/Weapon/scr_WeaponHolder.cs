using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface scr_WeaponHolder
{
    public bool HasWeapon();
    public void SetWeapon(scr_BaseWeapon weapon);
    public scr_BaseWeapon GetWeapon();
    public void DropWeapon();

}
