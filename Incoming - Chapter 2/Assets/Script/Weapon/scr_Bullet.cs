using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static scr_Models;

public class scr_Bullet : MonoBehaviour
{
    private Vector3 currentPos;
    private Vector3 currentVel;
    private Vector3 newPos;
    private Vector3 newVel;
    private scr_GunSO scr_gunSO;
    private bool IsInitialized;
    [Header("Setting")]
    public float LifeTime;
    private void Awake()
    {
        Destroy(gameObject, LifeTime);
    }
    void FixedUpdate()
    {
        MoveBulletOneStep();
    }

    void MoveBulletOneStep()
    {
        if (!IsInitialized) return;
        //Use an integration method to calculate the new position of the bullet
        float timeStep = Time.fixedDeltaTime;

        Heuns(timeStep, currentPos, currentVel, transform.up, scr_gunSO, out newPos, out newVel);

        //Debug.DrawRay(transform.position, transform.up * 5f);

        //Set the new values to the old values for next update
        currentPos = newPos;
        currentVel = newVel;

        //Add the new position to the bullet
        transform.position = currentPos;

        //Change so the bullet points in the velocity direction
        transform.forward = currentVel.normalized;
    }
    //Set start values when we create the bullet
    public void SetStartValues(scr_GunSO _scr_GunSO,Vector3 startPos, Vector3 startDir)
    {
        IsInitialized = true;
        scr_gunSO = _scr_GunSO;
        currentPos = startPos;
        currentVel = scr_gunSO.BulletSpeed * startDir;

        transform.position = startPos;
        transform.forward = startDir;
    }
}