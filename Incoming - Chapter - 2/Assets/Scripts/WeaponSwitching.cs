using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{

    public List<GameObject> weapons = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            weapons.Add(transform.GetChild(i).gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
