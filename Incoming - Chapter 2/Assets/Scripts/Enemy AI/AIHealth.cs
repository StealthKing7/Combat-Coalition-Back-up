using UnityEngine;

public class AIHealth: MonoBehaviour
{

    public float maxhealth;
    public float currenthealth;
    // Start is called before the first frame update
    void Awake()
    {
        currenthealth = maxhealth;
    }

    // Update is called once per frame
    public void TakeDamge(float amount)
    {
        currenthealth -= amount;
        if(currenthealth <= 0)
        {
            Died();
        }
    }
    void Died()
    {
        Debug.Log("Enemy Died");
    }
}
