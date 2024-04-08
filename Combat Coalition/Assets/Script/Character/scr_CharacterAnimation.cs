using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static scr_Models;

public class scr_CharacterAnimation : MonoBehaviour
{
    private Animator animator;
    private const string VELOCITY_X = "Velocity X";
    private const string VELOCITY_Z = "Velocity Z";
    private const string IS_SPRINTING = "IsSprinting";
    private const string JUMP = "Jump";
    [SerializeField] scr_CharacterController _CharacterController;
    private scr_BaseWeapon weapon;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        _CharacterController.CharacterMovementAnimationEvent += _CharacterController_CharacterAnimationEvent;
        _CharacterController.OnStanceChanged += _CharacterController_OnStanceChanged;
        _CharacterController.WeaponController.OnWeaponEquiped += WeaponSetUp;
        _CharacterController.CharacterJumpAnimationEvent += _CharacterController_CharacterJumpAnimationEvent;
    }

    private void _CharacterController_CharacterJumpAnimationEvent(object sender, System.EventArgs e)
    {
        animator.SetTrigger(JUMP);
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
        weapon = e.weapon;
        SetGunStyle(weapon.GetScr_WeaponSO().Is2Handed);
        switch (weapon.GetScr_WeaponSO().WeaponType)
        {
            case WeaponType.Gun:
                animator.SetLayerWeight(animator.GetLayerIndex("Gun Layer"), 1);
                animator.SetLayerWeight(animator.GetLayerIndex("Melee"), 0);
                break;
            case WeaponType.Melee:
                animator.SetLayerWeight(animator.GetLayerIndex("Melee"), 1);
                animator.SetLayerWeight(animator.GetLayerIndex("Gun Layer"), 0);
                break;
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
