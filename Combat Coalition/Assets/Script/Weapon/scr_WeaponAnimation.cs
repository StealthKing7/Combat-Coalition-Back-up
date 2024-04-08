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
    }
    private void Start()
    {
        characterController.CharacterMovementAnimationEvent += CharacterController_CharacterMovementAnimationEvent;
        characterController.WeaponController.OnWeaponEquiped += WeaponController_OnWeaponEquiped;
    }

    private void WeaponController_OnWeaponEquiped(object sender, scr_WeaponController.OnWeaponEquipedEventArgs e)
    {
        animator.runtimeAnimatorController = e.weapon.GetScr_WeaponSO().controller;
        Transform[] childrens = e.weapon.GetComponentsInChildren<Transform>();
        LeftHand.data.target = childrens.FirstOrDefault(child => child.name == "LeftHand");
        RightHand.data.target = childrens.FirstOrDefault(child => child.name == "RightHand");
        LeftHand.data.hint = childrens.FirstOrDefault(child => child.name == "LeftElbow");
        RightHand.data.hint = childrens.FirstOrDefault(child => child.name == "RightElbow");
        GetComponent<RigBuilder>().Build();
    }
    private void CharacterController_CharacterMovementAnimationEvent(object sender, scr_CharacterController.CharacterMovementAnimationEventArgs e)
    {
        animator.SetFloat(VELOCITY_X, e.dir.x);
        animator.SetFloat(VELOCITY_Z, e.dir.z);
    }
}
