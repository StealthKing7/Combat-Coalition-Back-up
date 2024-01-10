using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_GameManeger : MonoBehaviour
{
    public static scr_GameManeger Instance {  get; private set; }
    public  scr_WeaponSO _WeaponSO; 
    private List<scr_Attachment_SO> attachment_SOs = new List<scr_Attachment_SO>();
    private List<scr_CharacterController> Players = new List<scr_CharacterController>();

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
