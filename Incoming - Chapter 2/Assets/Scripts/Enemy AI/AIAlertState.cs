using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIAlertState : AIState
{
    private NavMeshAgent Agent;
    private bool IsInAttackRange;
    [SerializeField]private AIAttachState attachState;
    [SerializeField]
    private float range;
    [SerializeField]private Transform player;
    public override AIState RunCurrentState()
    {
        if (IsInAttackRange)
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
        Agent = GetComponentInParent<NavMeshAgent>();
            
    }
    private void Update()
    {
        if(Vector3.Distance(transform.position, player.position) < range)
        {
            IsInAttackRange = true;
        }
        
    }
    public void Follow()
    {
        Agent.SetDestination(player.position);
    }
}
            
    



   
    
