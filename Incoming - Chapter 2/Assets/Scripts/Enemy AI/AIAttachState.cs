using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttachState : AIState
{

    [SerializeField] private float range;
    [SerializeField] private Transform firepoint;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float FireRate; 
    [SerializeField] private float Damge;
    private float nextTimeToFire = 0;

    public override AIState RunCurrentState()
    {
        if(Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / FireRate;
            Fire();
        }
        return this;

    }
    public void Fire()
    {
        Debug.Log("Fire");
        RaycastHit hit;
        if (Physics.Raycast(firepoint.position,firepoint.forward, out hit, range, layerMask))
        {
            Health health = hit.transform.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamge(Damge);
            }
        }
    }
}
