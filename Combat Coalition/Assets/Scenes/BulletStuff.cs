using System.Collections.Generic;
using UnityEngine;
using static scr_Models;
public class BulletStuff : MonoBehaviour
{
    [SerializeField] Transform Bulletprf;
    [SerializeField] Transform DebugObj;
    [SerializeField] float speed;
    [SerializeField] int iteration;
    [SerializeField] int index;
    private Vector3 start;
    private Vector3 forward;
    public List<BulletsWithPath> BulletsTransforms = new List<BulletsWithPath>();
    [SerializeField] Vector2 Wind;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            Shoot();
        }
        for (int i = 0; i < BulletsTransforms.Count; i++)
        {
            BulletsTransforms[i] = new BulletsWithPath
            {
                bullets = BulletsTransforms[i].bullets,
                path = BulletsTransforms[i].path,
                PathIndex = BulletsTransforms[i].PathIndex
            };
            if (BulletsTransforms[i].PathIndex != BulletsTransforms[i].path.Length)
            {
                BulletsTransforms[i].bullets.position = Vector3.MoveTowards(BulletsTransforms[i].bullets.position, BulletsTransforms[i].path[BulletsTransforms[i].PathIndex], speed * Time.deltaTime);
                if (BulletsTransforms[i].bullets.position == BulletsTransforms[i].path[BulletsTransforms[i].PathIndex])
                {
                    BulletsTransforms[i] = new BulletsWithPath
                    {
                        bullets = BulletsTransforms[i].bullets,
                        path = BulletsTransforms[i].path,
                        PathIndex = BulletsTransforms[i].PathIndex + 1
                    };
                }
            }
        }
    }

    void Shoot()
    {
        var bullet = Instantiate(Bulletprf);
        bullet.position = transform.position;
        BulletsTransforms.Add(new BulletsWithPath
        {
            bullets = bullet,
            path = BulletPath(iteration,transform.position,transform.forward.normalized,speed,Wind),
            PathIndex = 0
        });
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < BulletsTransforms.Count; i++)
        {
            for (int j = 0; j < BulletsTransforms[i].path.Length; j++)
            {
                Gizmos.DrawWireSphere(BulletsTransforms[i].path[j], 1);
                if (j != BulletsTransforms[i].path.Length - 1)
                    Debug.DrawLine(BulletsTransforms[i].path[j], BulletsTransforms[i].path[j + 1], Color.black, 2);
            }
        }
    }
}