using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIAlertState : AIState
{
    [HideInInspector]
    public Transform Player;
    public LayerMask HidableLayers;
    public EnemyLineOfSightChecker LineOfSightChecker;
    private NavMeshAgent Agent;
    public float dot;
    [SerializeField]
    private bool IsInAttackRange;
    [SerializeField]
    private AIAttachState attachState;
    [SerializeField]
    private Transform player;
    public Transform AI;
    public bool boolean;
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
        if (boolean)
        {
            Hide();
        }
    }
    public void Hide()
    {
        float dotproduct = Vector3.Dot(AI.position, player.position);
        dot = dotproduct;

    }
    
}
            
    



   
    
