using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class scr_MainMenu : MonoBehaviour
{

    [SerializeField] private Button Assualt;
    [SerializeField] private Button Shotgun;
    [SerializeField] private Button Melee;
    [SerializeField] private Button randomizeButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button togglePartsButton;
    private void Awake()
    {
        MainMenu();
    }
    void MainMenu()
    {
        Assualt.onClick.AddListener(() => {
            scr_GunAttachmentsSystem.Instance.ChangeWeapon(scr_GunAttachmentsSystem.Instance.WeaponSOList.Find(e => e.WeaponType == scr_Models.WeaponType.Gun));
        });
        Shotgun.onClick.AddListener(() => {
            //scr_GunAttachmentsSystem.Instance.ChangeWeapon(scr_GunAttachmentsSystem.Instance.WeaponSOList, scr_Models.WeaponType.Gun);
        });
        Melee.onClick.AddListener(() => {
            scr_GunAttachmentsSystem.Instance.ChangeWeapon(scr_GunAttachmentsSystem.Instance.WeaponSOList.Find(e => e.WeaponType == scr_Models.WeaponType.Melee));
        });

        randomizeButton.onClick.AddListener(() => {
            scr_GunAttachmentsSystem.Instance.RandomizeParts();
        });

        togglePartsButton.onClick.AddListener(() => {
            scr_WeaponAttachmentUI.Instance.ToggleVisibility();
        });

        playButton.onClick.AddListener(() =>
        {
            scr_GameManeger.Instance._WeaponSO = scr_GunAttachmentsSystem.Instance.GetWeaponBodySO();
            foreach (var i in scr_GunAttachmentsSystem.Instance.GetWeaponComplete().Save())
            {
                scr_GameManeger.Instance.AddAttachments(i);
            }
            scr_SceneManeger.Instance.LoadScene();
        });
    }

}
