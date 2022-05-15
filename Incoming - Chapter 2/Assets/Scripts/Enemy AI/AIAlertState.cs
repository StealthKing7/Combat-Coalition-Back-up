using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIAlertState : AIState
{
    [SerializeField] bool InAttackRange;
    [SerializeField] AIAttachState attachState;
    [SerializeField] Animator animator;
    private NavMeshAgent agent;
    public LayerMask HideAbleLayer;
    public EnemyLineOfSightChecker LineOfSightChecker;
    [Range(-1, 1)]
    public float HideSencitivity = 0;
    private Coroutine MovementCoroutine;
    private Collider[] colliders = new Collider[10];
    public override AIState RunCurrentState()
    {
        if (InAttackRange)
        {
            return attachState;
        }
        else
        {
            return this;
        }
    }
    private void Awake()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        LineOfSightChecker.OnGainSight += HandleGainSight;
        LineOfSightChecker.OnLoseSight += HandleLoseSight;
    }
    void HandleGainSight(Transform target)
    {
        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
        }
        MovementCoroutine = StartCoroutine(Hide(target));
    }
    void HandleLoseSight(Transform target)
    {
        if (MovementCoroutine != null)
        {
            StopCoroutine(MovementCoroutine);
        }
    }
    IEnumerator Hide(Transform Target)
    {
        while (true)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i] = null;
            }
            int hits = Physics.OverlapSphereNonAlloc(agent.transform.position, LineOfSightChecker.sphereCollider.radius, colliders, HideAbleLayer);
            System.Array.Sort(colliders, CollIderArraySortComarer);
            for (int i = 0; i < hits; i++)
            {
                if (NavMesh.SamplePosition(colliders[i].transform.position, out NavMeshHit hit, 0.9f, agent.areaMask))
                {
                    if (!NavMesh.FindClosestEdge(hit.position, out hit, agent.areaMask))
                    {
                        Debug.LogError($"Uanble to find closest edge to{ hit.position}");
                    }
                    if (Vector3.Dot(hit.normal, (Target.position - hit.position).normalized) < HideSencitivity)
                    {
                        agent.SetDestination(hit.position);
                        break;
                    }
                    else
                    {
                        if (NavMesh.SamplePosition(colliders[i].transform.position - (Target.position - hit.position).normalized * 2, out NavMeshHit hit2, 2f, agent.areaMask))
                        {
                            if (!NavMesh.FindClosestEdge(hit2.position, out hit2, agent.areaMask))
                            {
                                Debug.LogError($"Unable to find edge close to {hit2.position} (second attempt)");
                            }

                            if (Vector3.Dot(hit2.normal, (Target.position - hit2.position).normalized) < HideSencitivity)
                            {
                                agent.SetDestination(hit2.position);
                                break;
                            }
                        }
                    }
                }
            }
            yield return null;
        }
    }
    int CollIderArraySortComarer(Collider a, Collider b)
    {
        if (a == null && b != null)
        {
            return 1;
        }
        else if (a != null & b == null)
        {
            return -1;
        }
        else if (a == null && b == null)
        {
            return 0;
        }
        else
        {
            return Vector3.Distance(agent.transform.position, a.transform.position).CompareTo(Vector3.Distance(agent.transform.position, b.transform.position));
        }
    }
}
            
    



   
    
