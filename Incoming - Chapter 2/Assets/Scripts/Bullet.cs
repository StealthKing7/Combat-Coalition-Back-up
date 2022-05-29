using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;
    private Rigidbody rb;
    private float damage;
    public void Initialized(float _speed,float _damage)
    {
        speed = _speed;
        damage = _damage;
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
        AIHealth aIHealth = other.GetComponent<AIHealth>();
        if(aIHealth != null)
        {
            aIHealth.TakeDamge(damage);
        }
        Destroy(gameObject);
    }
}
