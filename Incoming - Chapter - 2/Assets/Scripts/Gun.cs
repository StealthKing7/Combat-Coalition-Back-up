using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Gun",menuName ="Weapon/Guns")] 
public class Gun : ScriptableObject
{
    public float FireRate;
    public float Range;
    public float Damge;
    public GameObject Bullet;
    public float BulletSpeed; 
    public float BulletLifeTime; 
    public LayerMask layerMask;
}
