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

    }


    #endregion
}


