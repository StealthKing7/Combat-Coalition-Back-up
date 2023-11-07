using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Melee : scr_BaseWeapon
{
    public override void Execute()
    {
        Attack();
    }
    private void Start()
    {
        scr_InputManeger.Instance.AimingInPressed += AimInPressed;
        scr_InputManeger.Instance.AimingInReleased += AimInReleased;
    }
    private void AimInPressed()
    {
        
    }
    private void AimInReleased()
    {
        
    }
    private void Attack()
    {
        Debug.Log("Attack");
    }
}
