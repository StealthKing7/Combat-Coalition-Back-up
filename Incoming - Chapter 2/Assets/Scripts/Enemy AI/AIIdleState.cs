using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIIdleState : AIState
{
    public bool PlayerSpotted;
    [SerializeField] AIAlertState alertState;
    [SerializeField] float Range;
    [SerializeField] Transform player;
    public override AIState RunCurrentState()
    {
        if (PlayerSpotted)
        {

            return alertState;
        }
        else
        {
            return this;
        }
    }
}
