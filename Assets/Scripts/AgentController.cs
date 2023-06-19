using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform[] _waypoints;

    private int _currentWaypointIndex = 0;


    private void Start()
    {
        _agent.destination = _waypoints[_currentWaypointIndex].position;
    }

    void Update()
    {
        if (Vector3.Distance(_waypoints[_currentWaypointIndex].position, transform.position) < 1f)
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= _waypoints.Length)
                _currentWaypointIndex = 0;

            _agent.destination = _waypoints[_currentWaypointIndex].position;
        }
    }
}