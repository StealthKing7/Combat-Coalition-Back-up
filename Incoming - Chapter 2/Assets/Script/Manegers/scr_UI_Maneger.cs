using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class scr_UI_Maneger : MonoBehaviour
{
    private bool IsSetup = false;
    public static scr_UI_Maneger Instance;
    [SerializeField] private Button Assualt;
    [SerializeField] private Button Shotgun;
    [SerializeField] private Button Melee;
    [SerializeField] private Button randomizeButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button togglePartsButton;
    private float CurrentRectileSize;
    private RectTransform Rectile;
    [Header("CrossHair")]
    [SerializeField] float MaxRectileSize;
    [SerializeField] float MinRectileSize;
    [SerializeField] float RectileSizeSmoothing;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        if(scr_SceneManeger.Instance.GetSceneIndex() == 0)
        MainMenu();
    }
    private void Start()
    {
        scr_SceneManeger.Instance.OnSceneChanged += Instance_OnSceneChanged;
    }

    private void Instance_OnSceneChanged(object sender, System.EventArgs e)
    {
        foreach (var Player in scr_GameManeger.Instance.GetPlayerList())
        {
            Player.OnFpsUpdateText += UpdateFPSText;
            CurrentRectileSize = MinRectileSize;
            if (Player.WeaponController.GetWeapon().GetScr_WeaponSO().WeaponType == scr_Models.WeaponType.Gun)
            {
                var gunso = Player.WeaponController.GetWeapon().GetScr_WeaponSO() as scr_GunSO;
                Rectile = Instantiate(gunso.Rectile);
            }
        }
    }
    private void LateUpdate()
    {
        if (!IsSetup) return;
        UpdateRectileSize();
    }
    void UpdateRectileSize()
    {
        if (scr_InputManeger.Instance.Input_Movement != Vector2.zero)
        {
            CurrentRectileSize = Mathf.Lerp(CurrentRectileSize, MaxRectileSize, RectileSizeSmoothing);
        }
        else
        {
            CurrentRectileSize = Mathf.Lerp(CurrentRectileSize, MinRectileSize, RectileSizeSmoothing);
        }
        if (Rectile != null)
        {
            Rectile.sizeDelta = new Vector2(CurrentRectileSize, CurrentRectileSize);
        }
    }
    void UpdateFPSText(object sender,scr_CharacterController.OnFpsUpdateTextEventArgs e)
    {
        foreach(var Player in scr_GameManeger.Instance.GetPlayerList())
        {
            Player.FPSText.text = e.FrameRate + " fps";
        }
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
            scr_SceneManeger.Instance.SetScene(1);
        });
    }

}
