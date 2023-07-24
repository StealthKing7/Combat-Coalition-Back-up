using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class scr_Gun : scr_BaseWeapon
{
    public scr_WeaponHolder holder { get; private set; }
    public void EquipWeapon(scr_WeaponHolder _Holder)
    {
        holder = _Holder;
        if (_Holder.HasWeapon())
        {
            _Holder.DropWeapon();
            DestroySelf();
        }
       _Holder.SetWeapon(this);

    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    public override void Shoot(Transform BulletSpawn)
    {
        Instantiate(GetWeaponSO().Bullet, BulletSpawn.position, Quaternion.identity);
    }

}
