using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class PlayerMovement : MonoBehaviour
{
    PlayerInput inputActions;
    public event EventHandler OnMovement;
    [SerializeField] CharacterController characterController;
    [SerializeField] float Speed;
    [SerializeField] float JumpHieght;
    [SerializeField] float Gravity;
    Vector3 velocity;
    private void Start()
    {
        inputActions = new PlayerInput();
        inputActions.PlayerMap.Enable();
        OnMovement += Movement;

    }
    private void Update()
    {

        if (inputActions.PlayerMap.Movement.IsPressed())
        {
            OnMovement(this,EventArgs.Empty);
        }
      
    } 

    private void Movement(object obj,EventArgs args)
    {
        Vector3 move = transform.right*inputActions.PlayerMap.Movement.ReadValue<Vector2>().x+transform.forward* inputActions.PlayerMap.Movement.ReadValue<Vector2>().y;
        characterController.Move(move+velocity*Speed*Time.deltaTime);
    }


}
