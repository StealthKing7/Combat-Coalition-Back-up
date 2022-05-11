using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIAlertState : AIState
{
    [SerializeField] bool InAttackRange;
    [SerializeField] AIAttachState attachState;
    private NavMeshAgent agent;
    private void Awake()
    {
        agent = GetComponentInParent<NavMeshAgent>();
    }
    public override AIState RunCurrentState()
    {
        FindCover();
        if (InAttackRange)
        {
            return attachState;
        }
        else
        {
            return this;
        }
    }

    void FindCover()
    {
        GameObject[] covers = GameObject.FindGameObjectsWithTag("Cover");
        if(covers.Length == 0)
        {
            return;
        }
        GameObject nearestCover = null;
        float shortestDis = Mathf.Infinity; 
        foreach(GameObject cover in covers)
        {
            float dis = Vector3.Distance(transform.position, cover.transform.position);
            if(dis < shortestDis)
            {
                shortestDis = dis;
                nearestCover = cover;
            }
        }
        if (nearestCover != null)
        {
            agent.destination = nearestCover.transform.position;
        }
    }
}
