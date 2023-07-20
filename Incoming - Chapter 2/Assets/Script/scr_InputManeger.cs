using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;

public class scr_InputManeger : MonoBehaviour
{
    public static scr_InputManeger Instance { get; private set; }
    DefaultInputs DefaultInput;
    scr_WeaponController currentWeapon;
    private Vector2 Input_Movement;
    private Vector2 Input_View;
    private bool isLeaningLeft, isLeaningRight;
    public Action Jump,Crouch,Prone,ToggleSprint,StopSprint,AimingInPressed, AimingInReleased;
    public void SetWeapon(scr_WeaponController scr_WeaponController)
    {
        currentWeapon = scr_WeaponController;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        //Player Input
        DefaultInput = new DefaultInputs();
        DefaultInput.Character.Movement.performed += e => Input_Movement = e.ReadValue<Vector2>();
        DefaultInput.Character.View.performed += e => Input_View = e.ReadValue<Vector2>();
        DefaultInput.Character.Jump.performed += e => Jump();
        DefaultInput.Character.Crouch.performed += e => Crouch();
        DefaultInput.Character.Prone.performed += e => Prone();
        DefaultInput.Character.Sprint.performed += e => ToggleSprint();
        DefaultInput.Character.SprintReleased.performed += e => StopSprint();
        DefaultInput.Character.LeanLeftPressed.performed += e => isLeaningLeft = true;
        DefaultInput.Character.LeanLeftReleased.performed += e => isLeaningLeft = false;
        DefaultInput.Character.LeanRightPressed.performed += e => isLeaningRight = true;
        DefaultInput.Character.LeanRightReleased.performed += e => isLeaningRight = false;
        //Weapon Input
        DefaultInput.Weapon.Fire2Pressed.performed += e => AimingInPressed();
        DefaultInput.Weapon.Fire2Released.performed += e => AimingInReleased();
        DefaultInput.Weapon.Fire1Pressed.performed += e => currentWeapon.IsShooting = true;
        DefaultInput.Weapon.Fire1Released.performed += e => currentWeapon.IsShooting = false;
        DefaultInput.Enable();
    }
    public bool GetIsLeaningLeft()
    {
        return isLeaningLeft;
    }
    public bool GetIsLeaningRight()
    {
        return isLeaningRight;
    }
    public Vector3 GetInput_Movement()
    {
        return Input_Movement;
    }
    public Vector3 GetInput_View()
    {
        return Input_View;
    }

}
