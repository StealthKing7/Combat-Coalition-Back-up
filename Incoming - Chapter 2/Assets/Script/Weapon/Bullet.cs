using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Setting")]
    public float LifeTime;


    private void Awake()
    {
        Destroy(gameObject,LifeTime);
    }
}
