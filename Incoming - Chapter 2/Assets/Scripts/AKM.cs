using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AKM : MonoBehaviour
{
    private PlayerInput PlayerInput;
    public Camera cam;
    public float Range;
    public LayerMask layer;
    public Transform FirePoint;
    public GameObject BulletPB;
    void Awake()
    {
        PlayerInput = new PlayerInput();
        PlayerInput.Player.Enable();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInput.Player.Fire.IsPressed())
        {
            Shoot();
        }
    }
    void Shoot()
    {
        Vector3 mousePos = Vector3.zero;
        Vector2 ScreenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = cam.ScreenPointToRay(ScreenCenter);
        if(Physics.Raycast(ray, out RaycastHit hit, Range, layer))
        {
            mousePos += hit.point;
            Vector3 aimDir = (mousePos - FirePoint.position).normalized;
            GameObject bullet = Instantiate(BulletPB, FirePoint.position, Quaternion.LookRotation(aimDir, Vector3.up));
            //bullet.GetComponent<Bullet>().Initialized(BulletSpeed);
            //Destroy(bullet, BulletLifeTime);
        }

    }
}
