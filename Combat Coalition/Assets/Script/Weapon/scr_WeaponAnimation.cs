using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class scr_WeaponAnimation : MonoBehaviour
{
    [SerializeField] scr_CharacterController characterController;
    private Animator animator;
    [SerializeField] TwoBoneIKConstraint RightHand;
    [SerializeField] TwoBoneIKConstraint LeftHand;
    private const string VELOCITY_X = "VelocityX";
    private const string VELOCITY_Z = "VelocityZ";
    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterController.WeaponController.OnWeaponEquiped += WeaponController_OnWeaponEquiped;
    }
    private void Start()
    {
        characterController.CharacterMovementAnimationEvent += CharacterController_CharacterMovementAnimationEvent;
    }

    private void WeaponController_OnWeaponEquiped(object sender, scr_WeaponController.OnWeaponEquipedEventArgs e)
    {
        animator.runtimeAnimatorController = e.weapon.GetScr_WeaponSO().controller;
        LeftHand.data.target = e.weapon.LeftHand;
        RightHand.data.target = e.weapon.RightHand;
        LeftHand.data.hint = e.weapon.LeftElbow;
        RightHand.data.hint = e.weapon.RightElbow;
        GetComponent<RigBuilder>().Build();
    }
    private void CharacterController_CharacterMovementAnimationEvent(object sender, scr_CharacterController.CharacterMovementAnimationEventArgs e)
    {
        animator.SetFloat(VELOCITY_X, e.dir.x);
        animator.SetFloat(VELOCITY_Z, e.dir.z);
    }
}
