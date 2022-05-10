using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIIdleState : AIState
{
    private bool PlayerSpotted;
    [SerializeField] AIAlertState petrolState;
    [SerializeField] float Range;
    [SerializeField] Transform player;
    public override AIState RunCurrentState()
    {
        if (PlayerSpotted)
        {
            return petrolState;
        }
        else
        {
            return this;
        }
    }
    private void Awake()
    {
        Idle();
    }
    private void FixedUpdate()
    {
        float dis = Vector3.Distance(transform.position, player.position);
        if (dis <= Range)
        {
            PlayerSpotted = true;
        }

    }
    void Idle()
    {

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
