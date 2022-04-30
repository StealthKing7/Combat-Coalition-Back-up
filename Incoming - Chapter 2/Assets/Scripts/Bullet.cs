using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;
    private Rigidbody rb;

    public void Initialized(float _speed)
    {
        speed = _speed;
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        rb.velocity = transform.forward * speed;
    }
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
