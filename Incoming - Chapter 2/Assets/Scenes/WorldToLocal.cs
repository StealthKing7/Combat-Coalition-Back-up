using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class WorldToLocal : MonoBehaviour
{
    Vector3 gunpos;
    public Transform Cam;
    public Transform Sight;
    public bool IsAiming;
    Vector3 AimVelocity;
    Vector3 AimPos;

    private void Start()
    {
        gunpos = transform.position;
    }
    private void Update()
    {
        Aim();
    }
    void Aim()
    {
        var AimTargetPos = gunpos;
        if (IsAiming)
        {
            AimTargetPos = Cam.position + (transform.position - Sight.position) + Cam.forward;
        }
        Debug.DrawLine(transform.position, AimTargetPos, Color.black);
        Debug.Log(AimTargetPos);
        AimPos = transform.position;
        AimPos = Vector3.SmoothDamp(AimPos, AimTargetPos, ref AimVelocity, 0.2f);
        transform.position = AimPos;
    }
}
