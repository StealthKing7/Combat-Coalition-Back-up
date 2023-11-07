using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class scr_UI_Maneger : MonoBehaviour
{
    private float CurrentRectileSize;
    [Header("References")]
    [SerializeField] scr_CharacterController player;
    [SerializeField] RectTransform Rectile;
    [SerializeField] Text FPSText;
    [Header("CrossHair")]
    [SerializeField] float MaxRectileSize;
    [SerializeField] float MinRectileSize;
    [SerializeField] float RectileSizeSmoothing;


    private void Start()
    {
        player.OnFpsUpdateText += UpdateFPSText;
        CurrentRectileSize = MinRectileSize;
    }

    private void LateUpdate()
    {
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
        Rectile.sizeDelta = new Vector2(CurrentRectileSize, CurrentRectileSize);
    }
    void UpdateFPSText(object sender,scr_CharacterController.OnFpsUpdateTextEventArgs e)
    {
        FPSText.text = e.FrameRate + " fps";
    }


}
