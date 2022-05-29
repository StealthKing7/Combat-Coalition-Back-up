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
            
            alertState.Follow();
            return alertState;
        }
        else
        {
            return this;
        }
    }
    private void Update()
    {
        if(Vector3.Distance(transform.transform.position, player.position) < Range)
        {
            PlayerSpotted = true;
        }
    }
}
