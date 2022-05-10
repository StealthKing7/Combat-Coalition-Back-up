using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIIdleState : AIState
{
    [SerializeField] bool PlayerSpotted;
    [SerializeField] AIAlertState petrolState;
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
