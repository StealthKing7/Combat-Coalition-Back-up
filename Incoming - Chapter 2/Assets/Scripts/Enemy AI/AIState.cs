using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState : MonoBehaviour
{
    private float nextTimeToFire = 0;
    public abstract AIState RunCurrentState();

}
