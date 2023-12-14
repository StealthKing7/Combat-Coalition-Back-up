using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scr_SceneManeger : MonoBehaviour
{
    public static scr_SceneManeger Instance { get; private set; }
    public event EventHandler OnSceneChanged;
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

    public void SetScene(int Index)
    {
        SceneManager.LoadScene(Index);
    }
    public int GetSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

}
