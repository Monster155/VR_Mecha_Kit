using System;
using System.Collections;
using System.Collections.Generic;
using CabinRotation;
using Movement;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [SerializeField] private LegsMovementBase _movement;
    [SerializeField] private CabinRotationBase _rotation;

    private float _moveSpeed = 1f;
    private float _rotateSpeed = 1f;

    private void FixedUpdate()
    {
        if (!Mathf.Approximately(_movement.Power.magnitude, 0f))
            transform.Translate(_movement.Power * _moveSpeed);
        if (!Mathf.Approximately(_rotation.Angle, 0f))
            transform.Rotate(0, _rotation.Angle * _rotateSpeed, 0);
        // Debug.LogError(_movement.Power + " " + _rotation.Angle);
    }
}