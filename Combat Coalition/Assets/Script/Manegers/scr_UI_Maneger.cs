using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class scr_UI_Maneger : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI FPSText;
    public static scr_UI_Maneger Instance;
    private float CurrentRectileSize;
    private RectTransform Rectile;
    [Header("Interact")]
    [SerializeField] GameObject InteractObj;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Slider slider;
    [Header("CrossHair")]
    [SerializeField] float MaxRectileSize;
    [SerializeField] float MinRectileSize;
    [SerializeField] float RectileSizeSmoothing;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }
    private void Start()
    {
        scr_GameManeger.Instance.OnFpsUpdateText += UpdateFPSText;
        scr_GameManeger.Instance.GetPlayerList().ForEach(p=> p.WeaponController.OnWeaponEquiped += WeaponController_OnWeaponEquiped);
    }

    private void WeaponController_OnWeaponEquiped(object sender, scr_WeaponController.OnWeaponEquipedEventArgs e)
    {
        CurrentRectileSize = MinRectileSize;
        if (e.weapon.GetScr_WeaponSO().WeaponType == scr_Models.WeaponType.Gun)
        {
            var gunso = e.weapon.GetScr_WeaponSO() as scr_GunSO;
            if (Rectile != null)
            {
                Destroy(Rectile.gameObject);
            }
            Rectile = Instantiate(gunso.Rectile, canvas.transform);
        }
    }

    public void Interact(scr_Pickable pickable,float holdTime)
    {
        InteractObj.SetActive(pickable);
        if (pickable == null) return;
        Debug.Log(holdTime);
        slider.value = holdTime;
        text.text = pickable.Weapon.GetScr_WeaponSO().WeaponName;
    }
    private void LateUpdate()
    {
        StartCoroutine(Delay());
    }
    IEnumerator Delay()
    {
        yield return null;
        UpdateRectileSize();
    }
    void UpdateRectileSize()
    {
        scr_GameManeger.Instance.GetPlayerList().ForEach(b =>
        {
            if (b.WeaponController.GetWeapon().GetScr_WeaponSO().WeaponType == scr_Models.WeaponType.Gun)
            {
                Rectile.gameObject.SetActive(!b.WeaponController.GetWeapon().IsAiming);
            }
            if (b.InputManeger.Input_Movement == Vector2.zero)
            {
                CurrentRectileSize = Mathf.Lerp(CurrentRectileSize, MinRectileSize, RectileSizeSmoothing);
                return;
            }
            CurrentRectileSize = Mathf.Lerp(CurrentRectileSize, MaxRectileSize, RectileSizeSmoothing);
        });
        if (Rectile != null)
        {
            Rectile.sizeDelta = new Vector2(CurrentRectileSize, CurrentRectileSize);
        }
    }
    void UpdateFPSText(object sender,scr_GameManeger.OnFpsUpdateTextEventArgs e)
    {
        FPSText.text = e.FrameRate + " fps";
    }
}
