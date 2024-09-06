using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class scr_CameraShake : MonoBehaviour
{
    [SerializeField] scr_WeaponController controller;
    private CinemachineVirtualCamera _cam;
    private float shkTimer;
    private float shkTimeTotal;
    private float startingIntesity;
    private bool ShouldCameraShake;
    private void Awake()
    {
        _cam = GetComponent<CinemachineVirtualCamera>();
    }
    private void Start()
    {
        StartCoroutine(Delay());
    }
    IEnumerator Delay()
    {
        yield return null;
        if (controller.GetWeapon().GetScr_WeaponSO().WeaponType == scr_Models.WeaponType.Gun)
            (controller.GetWeapon() as scr_Gun).OnShoot += Scr_CameraShake_OnShoot;
    }
    private void Scr_CameraShake_OnShoot(object sender, scr_Gun.OnShootEventArgs e)
    {
        ShouldCameraShake = true;
    }

    void Shake(float intesity,float time)
    {
        CinemachineBasicMultiChannelPerlin multiChannelPerlin = _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        multiChannelPerlin.m_AmplitudeGain = intesity;
        startingIntesity = intesity;
        shkTimer = time;
        shkTimeTotal = time;
        ShouldCameraShake = false;
    }
    private void LateUpdate()
    {
        if (ShouldCameraShake)
        {
            Shake((controller.GetWeapon().GetScr_WeaponSO() as scr_GunSO).CameraShake, (controller.GetWeapon().GetScr_WeaponSO() as scr_GunSO).CameraShakeSmoothing);
        }
        if (shkTimer > 0)
        {
            shkTimer -= Time.deltaTime;
            CinemachineBasicMultiChannelPerlin multiChannelPerlin = _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            multiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntesity, 0, 1 - (shkTimer / shkTimeTotal));
        }
    }
}
