using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class scr_GameManeger : MonoBehaviour
{
    public static scr_GameManeger Instance {  get; private set; }
    public  scr_WeaponSO _WeaponSO; 
    public List<scr_Attachment_SO> attachment_SOs = new List<scr_Attachment_SO>();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }
}
