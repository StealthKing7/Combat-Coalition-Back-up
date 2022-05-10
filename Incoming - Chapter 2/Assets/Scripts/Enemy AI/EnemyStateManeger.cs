using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManeger : MonoBehaviour
{
   public AIState currentState;
    // Update is called once per frame
    void Update()
    {
        RunStateMachine();
    }
    void RunStateMachine()
    {
        AIState nextState = currentState?.RunCurrentState();
        if (nextState != null)
        {
            SwitchToNextState(nextState);
        }

    }
    void SwitchToNextState(AIState nextstate)
    {
        currentState = nextstate;
    }
}
