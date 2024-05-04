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
        animator.Rebind();
        GetComponent<RigBuilder>().Build();
        if (e.weapon.GetScr_WeaponSO().WeaponType == scr_Models.WeaponType.Gun) (e.weapon as scr_Gun).OnReload += Scr_WeaponAnimation_OnReloadChange;
        if (e.weapon.GetScr_WeaponSO().WeaponType == scr_Models.WeaponType.Gun) (e.weapon as scr_Gun).Aim_Reload += Scr_WeaponAnimation_Aim_Reload;
    }

    private void Scr_WeaponAnimation_Aim_Reload(object sender, scr_Gun.Aim_ReloadEventArgs e)
    {
        animator.SetLayerWeight(animator.GetLayerIndex("Reload"), e.Weight);
        animator.SetFloat("Reload_Multiplier", e.Weight);

    }

    private void Scr_WeaponAnimation_OnReloadChange(object sender, scr_Gun.OnReloadEventArgs e)
    {
        animator.SetTrigger("Reload");
        StartCoroutine(ReloadDelay(e.delay));
    }
    IEnumerator ReloadDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.ResetTrigger("Reload");
    }
    private void CharacterController_CharacterMovementAnimationEvent(object sender, scr_CharacterController.CharacterMovementAnimationEventArgs e)
    {
        animator.SetFloat(VELOCITY_X, e.dir.x);
        animator.SetFloat(VELOCITY_Z, e.dir.z);
    }
}
