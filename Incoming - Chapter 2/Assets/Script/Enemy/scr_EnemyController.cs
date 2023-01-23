using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using static scr_Models;

public class scr_EnemyController : MonoBehaviour
{
    private Vector3 StartingPosition;
    private Vector3 TargetPos;
    private NavMeshAgent navMeshAgent;
    [Header("Settings")]
    [SerializeField] EnemySettings EnemySettings;


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = EnemySettings.Speed;
        StartingPosition = transform.position;
        TargetPos = RoamingPos();
    }
    private void Update()
    {
        CalculateRoaming();
    }

    void CalculateRoaming()
    {
        if ((transform.position - TargetPos).sqrMagnitude < 1)
        {
            TargetPos = RoamingPos();
        }
        navMeshAgent.SetDestination(TargetPos);
    }

    Vector3 RoamingPos()
    {
        return StartingPosition + GetRoamingDir() * UnityEngine.Random.Range(-EnemySettings.RoamingRange, EnemySettings.RoamingRange);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, EnemySettings.Range);
    }
}
