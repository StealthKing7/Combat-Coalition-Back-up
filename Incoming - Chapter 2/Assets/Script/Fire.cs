using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Fire : MonoBehaviour
{
    PlayerInput inputs;
    public event EventHandler OnShooting;
    [SerializeField] float FireRate;
    [SerializeField] float MaxAmmo;
    [SerializeField] float Damge; 
    [SerializeField] float Range; 
    float NextTimeToFire = 0;
    float currentAmmo;
    private void Awake()
    {
        inputs = new PlayerInput();
        inputs.PlayerMap.Enable();
        OnShooting += Shoot;
    }
    private void Start()
    {
        currentAmmo = MaxAmmo;
    }
    private void Update()
    {
        if (inputs.PlayerMap.Shoot.IsPressed()&&Time.time>=NextTimeToFire&&currentAmmo!=0)
        {
            OnShooting(this, EventArgs.Empty);
            NextTimeToFire = Time.time + 1 / FireRate;
        }
    }
    void Shoot(object obj,EventArgs args)
    {
        currentAmmo--;
        Debug.Log("Shot");

    }
}
