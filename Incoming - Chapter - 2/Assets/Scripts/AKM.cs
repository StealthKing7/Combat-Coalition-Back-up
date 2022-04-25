#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AKM : MonoBehaviour
{
    private Camera cam;
    public float Range;
    public float Damge;
    public GameObject Bullet;
    public Transform firePoint;
    public LayerMask layerMask;
    public float BulletLifeTime;
    private PlayerInput playerInput;
    public float BulletSpeed;
    public Transform Test;
    private void Awake()
    {
        playerInput = new PlayerInput();
        cam = GetComponentInParent<Camera>();
        playerInput.Player.Enable();
        playerInput.Player.Fire.performed += Shoot;
    }
    void Update()
    {

    }
   void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Shot!");
            Vector3 mousePos = Vector3.zero;
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = cam.ScreenPointToRay(screenCenter);
            if (Physics.Raycast(ray, out RaycastHit hit, Range, layerMask))
            {
                mousePos = hit.point;
                Test.position = hit.point;
                Vector3 aimDir = (mousePos - firePoint.position).normalized;
                GameObject bullet = Instantiate(Bullet, firePoint.position, Quaternion.LookRotation(aimDir, Vector3.up));
                bullet.GetComponent<Bullet>().Initialized(BulletSpeed);
                Destroy(bullet, BulletLifeTime);
            }
        }
    }
}
