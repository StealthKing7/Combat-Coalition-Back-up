using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitching : MonoBehaviour
{
    private PlayerInput playerInput;
    public List<GameObject> weapons = new List<GameObject>();

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Enable();
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
