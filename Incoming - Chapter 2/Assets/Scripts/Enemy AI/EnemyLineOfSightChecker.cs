using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyLineOfSightChecker : MonoBehaviour
{
    public SphereCollider sphereCollider;
    public float fieldofview;
    public LayerMask LineofsightLayer;

    public delegate void GainSightEvent(Transform target);
    GainSightEvent OngainSight;
    public delegate void LoseSightEvent(Transform target);
    GainSightEvent OnLoseSight;

    private Coroutine CheckLineOfSightCoroutine;
    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!CheckLineOfSight(other.transform))
        {
            CheckLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(other.transform));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        OnLoseSight?.Invoke(other.transform);
        if (CheckLineOfSightCoroutine != null)
        {
            StopCoroutine(CheckForLineOfSight(other.transform));
        }
    }
    bool CheckLineOfSight(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, direction);
        if (dotProduct >= fieldofview)
        {
            if(Physics.Raycast(transform.position,direction,out RaycastHit hit, sphereCollider.radius, LineofsightLayer))
            {
                OngainSight?.Invoke(target);
                return true;
            }
        }
        return false;
    }
    IEnumerator CheckForLineOfSight(Transform target)
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        while (!CheckLineOfSight(target))
        {
            yield return wait;
        }
    }
}
