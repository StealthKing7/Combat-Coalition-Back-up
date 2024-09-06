using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static scr_Models;
public class BulletStuff : MonoBehaviour
{
    [SerializeField] Transform Bulletprf;
    [SerializeField] Transform DebugObj;
    [SerializeField] float speed;
    [SerializeField] int iteration;
    private Vector3 HitPoint;
    public List<Vector3> path = new List<Vector3>();
    private struct BulletWithDir
    {
        public Transform bullet;
        public Vector3 target; 
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            Shoot();
            foreach (var item in BulletPath(HitPoint, transform, iteration, speed))
            {
                path.Add(item);
            }
        }
        DebugObj.position = HitPoint;
        for (int i = 0; i < path.Count; i++)
        {
            if (i == path.Count-1) return;
            Debug.DrawLine(path[i], path[i + 1], Color.black, 2);
        }
    }
    void Shoot()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, float.MaxValue))
        {
            HitPoint =  hitInfo.point;
        }
    }
    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < path.Count; i++)
        {
            Gizmos.DrawWireSphere(path[i], 2);
        }
    }
}
