using System;
using UnityEngine;
using static scr_Models;

public class scr_Bullet : MonoBehaviour
{
    public Vector3 currentPos { get; private set; }
    public Vector3 currentVel { get;private set; }
    public Vector3 newPos { get; set; }
    public Vector3 newVel { get; set; }
    private scr_GunSO scr_gunSO;
    private bool _IsInitialized;
    public event EventHandler<OnHitEventArgs> OnHit;
    [Header("Setting")]
    [SerializeField] private float LifeTime;
    public class OnHitEventArgs:EventArgs
    {
        public GameObject hitObject;
        public Vector3 hitPoint;
    }


    private void Awake()
    {
        Destroy(gameObject, LifeTime);
    }
    void FixedUpdate()
    {
        if (!_IsInitialized) return;
        //Use an integration method to calculate the new position of the bullet
        float timeStep = Time.fixedDeltaTime;

        //Debug.DrawRay(transform.position, transform.up * 5f);

        //Set the new values to the old values for next update
        currentPos = newPos;
        currentVel = newVel;

        //Add the new position to the bullet
        transform.position = currentPos;

        //Change so the bullet points in the velocity direction
        transform.forward = currentVel.normalized;
        Debug.DrawRay(transform.position, transform.forward, Color.black, 5);
    }
    public void SetStartValues(scr_GunSO _scr_GunSO, Vector3 startPos, Vector3 startDir)
    {
        _IsInitialized = true;
        scr_gunSO = _scr_GunSO;
        currentPos = startPos;
        currentVel = scr_gunSO.BulletSpeed * startDir;

        transform.position = startPos;
        transform.forward = startDir;
    }
    public bool IsInitialized()
    {
        return _IsInitialized;
    }
    private void OnTriggerEnter(Collider other)
    {

        OnHit?.Invoke(this, new OnHitEventArgs
        {
            hitObject = other.gameObject,
            hitPoint = other.transform.position
        });
        Destroy(gameObject);
    }
}