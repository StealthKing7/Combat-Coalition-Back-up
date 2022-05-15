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
            
            return petrolState;
        }
        else
        {
            return this;
        }
    }
}
