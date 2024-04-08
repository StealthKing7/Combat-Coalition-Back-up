using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_InputManeger : MonoBehaviour
{
    DefaultInputs DefaultInput;
    public Vector2 Input_Movement { get; private set; }
    public Vector2 Input_View { get; private set; }
    public bool IsInteractPressed {  get;  set; }
    public bool IsLeaningLeft { get; private set; }
    public bool IsLeaningRight { get; private set; }
    public bool RightClick { get;  set; }
    public Action FireType { get; set; }
    public Action Jump { get;  set; }
    public Action Crouch { get; set; }
    public Action Prone { get; set; }
    public Action ToggleSprint { get; set; }
    public Action Reload { get; set; }
    public Action StopSprint { get; set; }
    public Action AimingInPressed { get; set; }
    public Action AimingInReleased { get; set; }
    private void Awake()
    {
        //Player Input
        DefaultInput = new DefaultInputs();
        DefaultInput.Character.Movement.performed += e => Input_Movement = e.ReadValue<Vector2>();
        DefaultInput.Character.View.performed += e => Input_View = e.ReadValue<Vector2>();
        DefaultInput.Character.Jump.performed += e => Jump();
        DefaultInput.Character.Crouch.performed += e => Crouch();
        DefaultInput.Character.Prone.performed += e => Prone();
        DefaultInput.Character.Sprint.performed += e => ToggleSprint();
        DefaultInput.Character.SprintReleased.performed += e => StopSprint();
        DefaultInput.Character.LeanLeftPressed.performed += e => IsLeaningLeft = true;
        DefaultInput.Character.LeanLeftReleased.performed += e => IsLeaningLeft = false;
        DefaultInput.Character.LeanRightPressed.performed += e => IsLeaningRight = true;
        DefaultInput.Character.LeanRightReleased.performed += e => IsLeaningRight = false;
        DefaultInput.Character.InteractPressed.performed += e => IsInteractPressed = true;
        DefaultInput.Character.InteractReleased.performed += e => IsInteractPressed = false;
        //Weapon Input
        DefaultInput.Weapon.Fire2Pressed.performed += e => AimingInPressed?.Invoke();
        DefaultInput.Weapon.Fire2Released.performed += e => AimingInReleased?.Invoke();
        DefaultInput.Weapon.Fire1Pressed.performed += e => RightClick = true;
        DefaultInput.Weapon.Fire1Released.performed += e => RightClick = false;
        DefaultInput.Weapon.FireType.performed += e => FireType();
        DefaultInput.Weapon.Reload.performed += e => Reload();
        DefaultInput.Enable();
    }
}
