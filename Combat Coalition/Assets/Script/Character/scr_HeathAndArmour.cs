using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static scr_Models;
public class scr_HeathAndArmour : MonoBehaviour
{
    public int CurrentArmour = 0;
    public int MaxArmour = 3;
    public float Health;
    public float CurrentAmrourHealth;
    private void Start()
    {
        Health = BasePlayerHealth;
        CurrentAmrourHealth = ArmourHeath * CurrentArmour;
    }
    private void Update()
    {
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void OnHit(float damage)
    {
        Debug.Log("Hit");
        if (CurrentAmrourHealth > 0)
        {
            CurrentAmrourHealth -= damage;
            return;
        }else if (CurrentAmrourHealth < 0)
        {
            Health += CurrentAmrourHealth;
            CurrentAmrourHealth = 0;
        }
        Health -= damage;
    }
}
