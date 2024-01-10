using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class scr_SceneManeger : MonoBehaviour
{
    public static scr_SceneManeger Instance { get; private set; }
    public event EventHandler OnSceneChanged;
    [SerializeField] AssetReference LoadingAssetReference;
    [SerializeField] AssetReference LevelAssetReference;
    private void Awake()
    {
        Instance = this; 
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        OnSceneChanged?.Invoke(this, EventArgs.Empty);
    }
    public void LoadScene()
    {
        LoadingAssetReference.LoadSceneAsync(LoadSceneMode.Single).Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var asyncOp = LevelAssetReference.LoadSceneAsync(LoadSceneMode.Single);
                asyncOp.Completed += (handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        handle.Result.ActivateAsync();
                    }
                };
                if (asyncOp.IsValid())
                {
                    float percent = asyncOp.GetDownloadStatus().Percent;
                    Debug.Log(percent * 100);
                }
            }
        };
    }
    public int GetSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

}
