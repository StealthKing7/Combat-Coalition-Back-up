using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    private float MaxHealth = 100f;
    private float currentHeath; 


    // Start is called before the first frame update
    void Start()
    {
        currentHeath = MaxHealth;
    }

    public void TakeDamge(float amount)
    {
        currentHeath -= amount;
        if(currentHeath < 0)
        {
            Died();
        }
    }
    void Died()
    {
        Debug.Log("Died");
    }
}
