#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField]
    private Gun gunAsset;
    public Transform firePoint;
    private Camera cam;
    private InputAction shoot;
    private InputAction Scope;
    private void Awake()
    {
        cam = GetComponentInParent<Camera>();
    }
    void Update()
    {
        if (shoot.enabled)
        {
            Shoot();
        }
    }
    void Shoot()
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

    }
}
