using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_WeaponAnimation : MonoBehaviour
{
    [SerializeField] scr_CharacterController characterController;
    private Animator animator;

    private const string VELOCITY_X = "VelocityX";
    private const string VELOCITY_Z = "VelocityZ";
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        characterController.CharacterMovementAnimationEvent += CharacterController_CharacterMovementAnimationEvent;
        characterController.WeaponController.OnWeaponEquiped += WeaponController_OnWeaponEquiped;
    }

    private void WeaponController_OnWeaponEquiped(object sender, scr_WeaponController.OnWeaponEquipedEventArgs e)
    {
        animator.runtimeAnimatorController = e.weapon.GetScr_WeaponSO().controller;
    }

    private void CharacterController_CharacterMovementAnimationEvent(object sender, scr_CharacterController.CharacterMovementAnimationEventArgs e)
    {
        animator.SetFloat(VELOCITY_X, e.dir.x);
        animator.SetFloat(VELOCITY_Z, e.dir.z);
    }
}
