using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static scr_Models;
public class scr_EnemyFSM : scr_EnemyBase
{
    private float IdleTimeRemaing;
    private int CurrentPatrolPoint;
    private NavMeshAgent agent;
    private Vector3 LastHeardLocation;
    private Transform LastSeenEnemy;
    [Header("Settings")]
    [SerializeField] EnemySettings EnemySettings; 
    [SerializeField] EState CurrentState;
    [SerializeField] List<Transform> patrolPoints;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = EnemySettings.Speed;
    }

    private void Start()
    {
        SetUpState(CurrentState);
    }

    private void Update()
    {
        UpdateState();
    }

    void SetUpState(EState newEState)
    {
        if(newEState == EState.Idle)
        {
            IdleTimeRemaing = Random.Range(EnemySettings.IldeMinTime, EnemySettings.IldeMaxTime);
        }
        else if(newEState == EState.Patrol)
        {
            CurrentPatrolPoint = Random.Range(0, patrolPoints.Count);
        }
        else if(newEState == EState.AlertAudio)
        {
            transform.rotation = Quaternion.LookRotation(LastHeardLocation - transform.position, Vector3.up);
        }
        else if(newEState == EState.AlertVisual)
        {
            transform.LookAt(LastSeenEnemy, Vector3.up);
        }
        CurrentState = newEState;
        Debug.Log("Current State is " + CurrentState.ToString());
    }
    void UpdateState()
    {
        if (CurrentState == EState.Idle)
        {
            Idle();
        }
        else if (CurrentState == EState.Patrol)
        {
            Patrol();
        }
        else if (CurrentState == EState.AlertAudio)
        {
            AudioAlert();
        }
        else if (CurrentState == EState.AlertVisual)
        {
            VisualAlert();
        } 
    }
    void Idle()
    {
        IdleTimeRemaing -= Time.deltaTime;
        if (IdleTimeRemaing <= 0)
        {
            PickNewState();
        }
    }
    void Patrol()
    {
        Vector3 vectorToPatrolPoint = patrolPoints[CurrentPatrolPoint].position - transform.position;

        // reached patrol point?
        if (vectorToPatrolPoint.magnitude <= EnemySettings.PatrollingReachedThreshHold)
        {
            // advance to the next point
            CurrentPatrolPoint = (CurrentPatrolPoint + 1) % patrolPoints.Count;
        }

        // move towards the point
        agent.SetDestination(patrolPoints[CurrentPatrolPoint].position);
    }
    void AudioAlert()
    {

    }
    void VisualAlert()
    {
        transform.LookAt(LastSeenEnemy, Vector3.up);
    }
    void PickNewState()
    {
        if (CurrentState == EState.Idle)
            SetUpState(EState.Patrol);
        else if (CurrentState == EState.Patrol)
            SetUpState(EState.Idle);
    }
}
