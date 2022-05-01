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
    public float BulletSpeed;
    public float BulletLifeTime;
    private Recoil recoil;
    public float FireRate;
    private float NextTimeToFire = 0;
    private Animator animator;
    private bool isScoped;
    void Awake()
    {
        recoil = GetComponentInChildren<Recoil>();  
        PlayerInput = new PlayerInput();
        PlayerInput.Player.Enable();
        animator = cam.GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInput.Player.Fire.IsPressed() && Time.time >= NextTimeToFire)
        {
            NextTimeToFire = Time.time + 1f / FireRate;
            recoil.Fire();
            Shoot();
        }
        if (PlayerInput.Player.Scope.triggered)
        {
            isScoped = !isScoped;
            animator.SetBool("Scope", isScoped);
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
            bullet.GetComponent<Bullet>().Initialized(BulletSpeed);
            Destroy(bullet, BulletLifeTime);
        }

    }
}
