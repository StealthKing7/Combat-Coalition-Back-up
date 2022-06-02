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
    private float maxammo = 30f;
    private float currentammo;
    public override AIState RunCurrentState()
    {
        return this;

    }
    private void Awake()
    {
        currentammo = maxammo;
    }
    void Update()
    {
        if ((Vector3.Distance(player.position, transform.position) <= Distancerange) && Time.time >= timetofire)
        {
            timetofire = Time.time + 1f / FireRate;
            Fire();         
        }
        if (currentammo <= 0)
        {
           StartCoroutine(Reload());
        }
    }
    void Fire()
    {
        currentammo--;
        RaycastHit hit;
        if (Physics.Raycast(firepoint.position,firepoint.forward, out hit, range, layerMask))
        {
            Health health = hit.transform.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamge(Damge);
                health.gameObject.GetComponentInChildren<CameraRecoil>().RecoilFire();
            }
        }
    }
    IEnumerator Reload()
    {
        yield return null;
    }
}
