using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class AKM : MonoBehaviour
{
    private PlayerInput PlayerInput;
    [SerializeField] private Camera cam;
    [SerializeField] private float Range;
    [SerializeField] private LayerMask layer;
    [SerializeField] private Transform FirePoint;
    [SerializeField] private GameObject BulletPB;
    [SerializeField] private float BulletSpeed;
    [SerializeField] private float BulletLifeTime;
    private Recoil recoil;
    [SerializeField] private float FireRate;
    private float NextTimeToFire = 0;
    private Animator animator;
    private bool isScoped;
    private CameraRecoil cameraRecoil;
    [SerializeField] private Text Ammo;
    [SerializeField] private float MaxAmmo;
    private float currentAmmo;
    [SerializeField] private float ReloadTime;
    private bool isReloading;
    [Space(10)]
    [Header("Camera Recoil")]
    [SerializeField] private float RecoilX;
    [SerializeField] private float RecoilY;
    [SerializeField] private float Snapiness;
    [SerializeField] private float ReturnSpeed;
    void Awake()
    {
        Application.targetFrameRate = 100;
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
        Ammo.text = currentAmmo.ToString();
        if (PlayerInput.Player.Scope.triggered)
        {
            isScoped = !isScoped;
            animator.SetBool("Scope", isScoped);
            recoil.aim = isScoped;
        }
        if (isScoped)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 45, 0.25f);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60, 0.25f);
        }
        if ((currentAmmo == 0) || (currentAmmo < MaxAmmo && PlayerInput.Player.Reload.triggered))
        {
            if (!isReloading)
            StartCoroutine(Reload());
        }
        if (PlayerInput.Player.Fire.IsPressed() && Time.time >= NextTimeToFire && currentAmmo > 0 && !isReloading)
        {
            NextTimeToFire = Time.time + 1f / FireRate;
            recoil.Fire();
            cameraRecoil.RecoilFire();
            Shoot();
        }
    }
    void Shoot()
    {

        currentAmmo--;
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
        isScoped = false;
        animator.SetBool("Scope", false);
        isReloading = true;
        string AnimatoinType;
        switch (currentAmmo)
        {
            case 0:
                AnimatoinType = "Reload_Empty";
                break;
            default:
                AnimatoinType = "Reload_NotEmpty";
                break;
        }
        Debug.Log("Reloading");
        animator.SetBool(AnimatoinType, true);
        yield return new WaitForSeconds(ReloadTime - 0.25f);
        animator.SetBool(AnimatoinType, false);
        yield return new WaitForSeconds(0.25f);
        currentAmmo = MaxAmmo;
        isReloading = false;
    } 
}
