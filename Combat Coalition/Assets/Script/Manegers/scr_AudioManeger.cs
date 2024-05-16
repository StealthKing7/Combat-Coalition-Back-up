using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class scr_AudioManeger : MonoBehaviour
{
    public static scr_AudioManeger Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public void PlayOneShot(EventReference sound, Vector3 pos)
    {
        RuntimeManager.PlayOneShot(sound, pos);
    }
}
