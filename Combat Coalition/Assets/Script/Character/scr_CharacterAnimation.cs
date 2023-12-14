using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static scr_Models;

public class scr_CharacterAnimation : MonoBehaviour
{
    private Animator animator;
    private const string VELOCITY_X = "Velocity X";
    private const string VELOCITY_Z = "Velocity Z";
    private const string IS_SPRINTING = "IsSprinting";
    private const string JUMP = "Jump";
    [Range(0, 1)]
    [SerializeField] float HandIKAmount;
    [Range(0, 1)]
    [SerializeField] float ElbowIKAmount;
    [SerializeField] scr_CharacterController _CharacterController;

    private Transform LeftElbowIKTarget;
    private Transform RightElbowIKTarget;
    private Transform LeftHandIKTarget;
    private Transform RightHandIKTarget;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        _CharacterController.CharacterMovementAnimationEvent += _CharacterController_CharacterAnimationEvent;
        _CharacterController.OnStanceChanged += _CharacterController_OnStanceChanged;
        _CharacterController.WeaponController.OnWeaponEquiped += WeaponSetUp;
        
    }

    private void _CharacterController_OnStanceChanged(object sender, scr_CharacterController.OnStanceChangedEventArgs e)
    {
        switch (e.stance)
        {
            case PlayerStance.Crouch:
                animator.SetBool("Crouch", true);
                animator.SetBool("Prone", false);
                break;
            case PlayerStance.Prone:
                animator.SetBool("Prone", true);
                animator.SetBool("Crouch", false);
                break;
            default:
                animator.SetBool("Crouch", false);
                animator.SetBool("Prone", false);
                break;
        }
    }

    void WeaponSetUp(object sender, scr_WeaponController.OnWeaponEquipedEventArgs e)
    {
        var weapon = e.weapon;
        SetGunStyle(weapon.GetScr_WeaponSO().Is2Handed);
        if (weapon.GetScr_WeaponSO().WeaponType == WeaponType.Gun)
        {
            animator.SetLayerWeight(1, 1);
            animator.SetLayerWeight(2, 0);
        }
        else if(weapon.GetScr_WeaponSO().WeaponType == WeaponType.Melee)
        {
            animator.SetLayerWeight(1, 0);
            animator.SetLayerWeight(2, 1);
        }
        Transform[] allChildren = e.weapon.gameObject.GetComponentsInChildren<Transform>();
        LeftElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftElbow");
        RightElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "RightElbow");
        LeftHandIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftHand");
        RightHandIKTarget = allChildren.FirstOrDefault(child => child.name == "RightHand");
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (!_CharacterController.WeaponController.HasWeapon())
        {
            animator.SetLayerWeight(1, 0);
            return;
        }

        if (LeftHandIKTarget != null)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, HandIKAmount);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, HandIKAmount);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIKTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandIKTarget.rotation);
        }
        if (RightHandIKTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, HandIKAmount);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, HandIKAmount);
            animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandIKTarget.rotation);
            animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandIKTarget.position);
        }
        if (LeftElbowIKTarget != null)
        {
            animator.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftElbowIKTarget.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, ElbowIKAmount);
        }

        if (RightElbowIKTarget != null)
        {
            animator.SetIKHintPosition(AvatarIKHint.RightElbow, RightElbowIKTarget.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, ElbowIKAmount);
        }
    }

    void SetGunStyle(bool Is2Handed)
    {
        animator.SetBool("Is2Handed", Is2Handed);
        animator.SetBool("Is1Handed", !Is2Handed);
    }

    private void _CharacterController_CharacterAnimationEvent(object sender, scr_CharacterController.CharacterMovementAnimationEventArgs e)
    {
        float x = e.dir.x;
        float z = e.dir.z;
        animator.SetBool(IS_SPRINTING, e.isSprinting);
        animator.SetFloat(VELOCITY_X, x);
        animator.SetFloat(VELOCITY_Z, z);
    }
}
