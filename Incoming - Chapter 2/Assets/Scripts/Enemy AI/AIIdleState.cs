using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIIdleState : AIState
{
    public bool PlayerSpotted;
    [SerializeField] AIAlertState petrolState;
    [SerializeField] float Range;
    [SerializeField] Transform player;
    public override AIState RunCurrentState()
    {
        if (PlayerSpotted)
        {
            petrolState.AlertEnemies();
            //petrolState.FindCover();
            return petrolState;
        }
        else
        {
            return this;
        }
    }
 

    private void FixedUpdate()
    {
        float dis = Vector3.Distance(transform.position, player.position);
        if (dis <= Range)
        {
            PlayerSpotted = true;
        }

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
