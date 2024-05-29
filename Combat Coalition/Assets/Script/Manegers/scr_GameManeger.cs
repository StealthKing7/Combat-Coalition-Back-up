using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static scr_CharacterController;
using UnityEngine.Playables;
using System;

public class scr_GameManeger : MonoBehaviour
{
    public event EventHandler<OnFpsUpdateTextEventArgs> OnFpsUpdateText;
    public class OnFpsUpdateTextEventArgs : EventArgs
    {
        public float FrameRate;
    }
    public static scr_GameManeger Instance {  get; private set; }
    public  List<scr_WeaponSO> AllWeaponSO; 
    private List<scr_Attachment_SO> attachment_SOs = new List<scr_Attachment_SO>();
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
    public void AddAttachments(scr_Attachment_SO scr_Attachment)
    {
        attachment_SOs.Add(scr_Attachment);
    }
    public List<scr_Attachment_SO> GetAttachments()
    {
        return attachment_SOs;
    }
}
