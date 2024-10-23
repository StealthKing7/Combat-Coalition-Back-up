using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class scr_MainMenu : MonoBehaviour
{

    [SerializeField] private Button Assualt;
    [SerializeField] private Button Shotgun;
    [SerializeField] private Button Melee;
    [SerializeField] private Button SelecteWeapon;
    [SerializeField] private Button randomizeButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button togglePartsButton;
    private scr_GunAttachmentsSystem gunAttachmentsSystem;
    private void Awake()
    {
        gunAttachmentsSystem = scr_GunAttachmentsSystem.Instance;
        MainMenu();
    }
    void MainMenu()
    {
        Assualt.onClick.AddListener(() => {
            gunAttachmentsSystem.ChangeWeapon(scr_GunAttachmentsSystem.Instance.WeaponSOList.Find(e => e.WeaponType == scr_Models.WeaponType.Gun));
        });
        Shotgun.onClick.AddListener(() => {
            //scr_GunAttachmentsSystem.Instance.ChangeWeapon(scr_GunAttachmentsSystem.Instance.WeaponSOList, scr_Models.WeaponType.Gun);
        });
        Melee.onClick.AddListener(() => {
            gunAttachmentsSystem.ChangeWeapon(scr_GunAttachmentsSystem.Instance.WeaponSOList.Find(e => e.WeaponType == scr_Models.WeaponType.Melee));
        });
        SelecteWeapon.onClick.AddListener(() =>
        {
            gunAttachmentsSystem.SelecteWeapon();
        });
        randomizeButton.onClick.AddListener(() => {
            gunAttachmentsSystem.RandomizeParts();
        });

        togglePartsButton.onClick.AddListener(() => {
            scr_WeaponAttachmentUI.Instance.ToggleVisibility();
        });

        playButton.onClick.AddListener(() =>
        {
            scr_GameManeger.Instance.AllWeaponSO = scr_GunAttachmentsSystem.Instance.GetWeaponBodySO();
            //scr_GunAttachmentsSystem.Instance.GetWeaponComplete().Save().ForEach(i => scr_GameManeger.Instance.AddAttachments(i));
            scr_SceneManeger.Instance.LoadScene();
        });
    }

}
