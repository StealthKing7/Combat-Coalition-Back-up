using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class scr_Models
{
    #region - Player -
    [System.Serializable]
    public class PlayerSettingModel
    {
        [Header("View Settings")]
        public float ViewXSencitivity;
        public float ViewYSencitivity;
        public bool ViewXInverted;
        public bool ViewYInverted;
        [Header("Movement Settings")]
        public float WalkingForwardSpeed;
        public float WalkingBackwardSpeed;
        public float WalkingStrafeSpeed;

        [Header("Jump Settings")]
        public float JumpingHeight;
        public float JumpingFallof;


    }


    #endregion
}


