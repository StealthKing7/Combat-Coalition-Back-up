using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAlertState : AIState
{
    [SerializeField] bool InAttackRange;
    [SerializeField] AIAttachState attachState;
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
}
