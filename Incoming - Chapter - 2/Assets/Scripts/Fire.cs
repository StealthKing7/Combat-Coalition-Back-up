#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    /*private WeaponSwitching switching;
    private Gun gunAsset;
    private GameObject gun;
    private void Awake()
    {
        switching = GetComponentInChildren<WeaponSwitching>();
        //cam = GetComponentInParent<Camera>();
    }
    void Update()
    {
        for (int i = 0; i < switching.weapons.Count; i++)
        {

            gun = switching.weapons[i].gameObject;
                
            
        }
    }
   /* void Shoot()
    {
        Vector3 mousePos = Vector3.zero;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = cam.ScreenPointToRay(screenCenter);
        if (Physics.Raycast(ray, out RaycastHit hit, gunAsset.Range, gunAsset.layerMask))
        { 
            mousePos += hit.point;
            Vector3 aimDir=(firePoint.position - hit.point).normalized;
            GameObject bullet = Instantiate(gunAsset.Bullet,firePoint.position, Quaternion.LookRotation(aimDir, Vector3.up));

            Destroy(bullet, gunAsset.BulletLifeTime);
        }

    }*/
}
