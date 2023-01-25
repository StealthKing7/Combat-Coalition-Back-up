using UnityEngine;
public class scr_EnemyBase : MonoBehaviour
{   
    public void OnEnemyDetected(GameObject enemy)
    {
        Debug.Log("[" + gameObject.name + "] Detected" + enemy.name);
    }
    public void OnEnemyLost(GameObject enemy)
    {
        Debug.Log("[" + gameObject.name + "] Lost " + enemy.name);
    }
    public void OnSoundHeard(Vector3 location)
    {
        Debug.Log("[" + gameObject.name + "] I heard Somthing at" + location);
    }
}
