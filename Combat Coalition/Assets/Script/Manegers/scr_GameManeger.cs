using System.Collections.Generic;
using UnityEngine;
using static scr_Models;
using System;

public class scr_GameManeger : MonoBehaviour
{
    public event EventHandler<OnFpsUpdateTextEventArgs> OnFpsUpdateText;
    public class OnFpsUpdateTextEventArgs : EventArgs
    {
        public float FrameRate;
    }
    public static scr_GameManeger Instance {  get; private set; }
    public  List<WeaponsWithAttachments> AllWeaponSO; 
    private List<scr_CharacterController> Players = new List<scr_CharacterController>();
    private float frameRate;
    private float FpsTimer;



    private void Update()
    {
        OnFpsUpdateText?.Invoke(this, new OnFpsUpdateTextEventArgs { FrameRate = frameRate });
        if (FpsTimer > 1f)
        {
            frameRate = (int)(1f / Time.unscaledDeltaTime);
            FpsTimer = 0f;
        }
        else
        {
            FpsTimer += Time.deltaTime;
        }
    }
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void AddPlayer(scr_CharacterController _player)
    {
        Players.Add(_player);
    }
    public List<scr_CharacterController> GetPlayerList()
    {
        return Players;
    }
}
