using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttachState : AIState
{

    [SerializeField] private float range;
    [SerializeField] private float Distancerange;
    [SerializeField] private Transform firepoint;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float FireRate; 
    [SerializeField] private float Damge;
    [SerializeField]private Transform player;
    private float timetofire = 0;

    public override AIState RunCurrentState()
    {
        return this;

    }
    void Update()
    {
        if ((Vector3.Distance(player.position, transform.position) <= Distancerange) && Time.time >= timetofire)
        {
            timetofire = Time.time + 1f / FireRate;
            Fire();         
        }
    }
    void Fire()
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
