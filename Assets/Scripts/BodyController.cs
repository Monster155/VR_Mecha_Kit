using System;
using CabinRotation;
using Movement;
using UnityEngine;

public class BodyController : MonoBehaviour
{
    [SerializeField] private Transform _cabinTransform;
    [SerializeField] private Transform _legsTransform;
    [Space]
    [SerializeField] private CabinRotationBase _cabinRotation;
    [SerializeField] private float _rotationScaler = 0.1f;
    [Space]
    [SerializeField] private LegsMovementBase _legsMovement;
    [SerializeField] private float _movementScaler = 0.1f;

    private void Update()
    {
        _cabinTransform.Rotate(new Vector3(0, _cabinRotation.Angle * _rotationScaler, 0));
        _legsTransform.Translate(_legsMovement.Power * _movementScaler);
    }
}