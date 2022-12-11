using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    PlayerInput inputActions;

    private void Start()
    {
        inputActions = new PlayerInput();
        inputActions.PlayerMap.Enable();
    }

    private void Update()
    {
        if (inputActions.PlayerMap.Shoot.IsPressed())
        {
            Shoot();
        }
    }
    void Shoot()
    {
        Debug.Log("Shoot");
    }
}
