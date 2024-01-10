using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_LoadingManeger : MonoBehaviour
{
    public static scr_LoadingManeger Instance { get; private set; }
    [SerializeField] private Slider LoadingBar;
    [SerializeField] private float LoadingProgress;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        scr_SceneManeger.Instance.OnSceneChanged += Instance_OnSceneChanged;
    }
    private void Instance_OnSceneChanged(object sender, System.EventArgs e)
    {
        if (scr_SceneManeger.Instance.GetSceneIndex() == 1)
        {
            LoadingBar.value = LoadingProgress;
        }
    }
    public void SetLoadingProgress(float progress)
    {
        LoadingProgress = progress;
    }
}
