using UnityEngine;
using System.Collections;
using UnityEngine.UI;
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
    private CameraRecoil cameraRecoil;
    public Text Ammo;
    public float MaxAmmo;
    private float currentAmmo;
    public float ReloadTime;
    [Space(10)]
    [Header("Camera Recoil")]
    public float RecoilX;
    public float RecoilY;
    public float Snapiness;
    public float ReturnSpeed;
    void Awake()
    {
        recoil = GetComponentInChildren<Recoil>();  
        PlayerInput = new PlayerInput();
        PlayerInput.Player.Enable();
        animator = cam.GetComponent<Animator>();
        cameraRecoil = GetComponentInParent<CameraRecoil>();
        cameraRecoil.CameraRecoilValues(RecoilX, RecoilY, Snapiness, ReturnSpeed);

    }
    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = MaxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInput.Player.Scope.triggered)
        {
            isScoped = !isScoped;
            animator.SetBool("Scope", isScoped);
            recoil.aim = isScoped;
        }
        Ammo.text = currentAmmo.ToString();
        if ((currentAmmo == 0) || (currentAmmo < MaxAmmo && PlayerInput.Player.Reload.triggered))
        {
            StartCoroutine(Reload());
        }
        if (currentAmmo == 0)
        {
            return;
        }
        if (PlayerInput.Player.Fire.IsPressed() && Time.time >= NextTimeToFire&&currentAmmo > 0)
        {
            NextTimeToFire = Time.time + 1f / FireRate;
            recoil.Fire();
            Shoot();
        }


    }
    void Shoot()
    {
        currentAmmo--;
        cameraRecoil.RecoilFire();
        Vector3 mousePos = Vector3.zero;
        Vector2 ScreenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = cam.ScreenPointToRay(ScreenCenter);
        if (Physics.Raycast(ray, out RaycastHit hit, Range, layer))
        {
            mousePos += hit.point;
            Vector3 aimDir = (mousePos - FirePoint.position).normalized;
            GameObject bullet = Instantiate(BulletPB, FirePoint.position, Quaternion.LookRotation(aimDir, Vector3.up));
            bullet.GetComponent<Bullet>().Initialized(BulletSpeed);
            Destroy(bullet, BulletLifeTime);
        }
        
    }
    IEnumerator Reload()
    {
        Debug.Log("Reloading");
        animator.SetBool("Reloading", true);
        yield return new WaitForSeconds(ReloadTime - 2.5f);
        animator.SetBool("Reloading", false);
        yield return new WaitForSeconds(2.5f);
        currentAmmo = MaxAmmo;
    } 
}
