using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttachState : AIState
{

    [SerializeField] private float range;
    //[SerializeField] private Transform firepoint;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float Damge;

    public override AIState RunCurrentState()
    {
        Fire();
        return this;

    }
    public void Fire()
    {
        Debug.Log("Fire");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, range, layerMask))
        {
            Health health = hit.transform.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamge(Damge);
            }
        }
    }
}
