using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttachState : AIState
{


    public override AIState RunCurrentState()
    {
        Debug.Log("Attack");
        return this;
    }
}
