using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Melee : scr_BaseWeapon
{
    public override void Execute()
    {
        Attack();
    }
    private void Attack()
    {
        Debug.Log("Attack");
    }
}
