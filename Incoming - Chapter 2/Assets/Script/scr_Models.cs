using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class scr_Models
{
    #region - Player -

    public enum PlayerStance
    {
        Stand,
        Crouch,
        Prone
    }
     
    [System.Serializable]
    public class PlayerSettingModel
    {
        [Header("View Settings")]
        public float ViewXSencitivity;
        public float ViewYSencitivity;
        public bool ViewXInverted;
        public bool ViewYInverted;
        [Header("Movement - Running")]
        public float RunningForwardSpeed; 
        public float RunningStrafeSpeed; 
        [Header("Movement - Settings")]
        public bool SprintHold;
        public float MovementSmoothing;
        [Header("Movement - Walking")]
        public float WalkingForwardSpeed;
        public float WalkingBackwardSpeed;
        public float WalkingStrafeSpeed;

        [Header("Jump Settings")]
        public float JumpingHeight;
        public float JumpingFallof;
        public float FallingSmoothing;
        [Header("Speed Effectors")]
        public float SpeedEffector = 1f;
        public float CrouchSpeedEffector;
        public float ProneSpeedEffector;
        public float FallingSpeedEffector;


    }
    [System.Serializable]
    public class CharacterStance
    {
        public float CameraHeight;
        public CapsuleCollider StanceCollider;
    }

    #endregion


    #region - Weapon -
    [System.Serializable]
    public class WeaponSettingsModel
    {
        [Header("Sway")]
        public float SwayAmount;
        public bool SwayYInverted; 
        public bool SwayXInverted; 
        public float SwaySmoothing;
    }

    #endregion
}


