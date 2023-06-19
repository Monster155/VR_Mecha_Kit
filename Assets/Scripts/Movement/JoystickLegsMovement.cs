using System;
using CabinRotation;
using UnityEngine;

namespace Movement
{
    public class JoystickLegsMovement : LegsMovementBase
    {
        [SerializeField] private Transform _joystickTransform;
        [SerializeField] private Axis _forwardAxis;
        [SerializeField] private Axis _sideAxis;
        [SerializeField] private float _deltaAngle;

        private void Update()
        {
            float joystickForwardAngle = _forwardAxis switch
            {
                Axis.X => _joystickTransform.rotation.eulerAngles.x,
                Axis.Y => _joystickTransform.rotation.eulerAngles.y,
                Axis.Z => _joystickTransform.rotation.eulerAngles.z,
                _ => 0
            };
            float joystickSideAngle = _sideAxis switch
            {
                Axis.X => _joystickTransform.rotation.eulerAngles.x,
                Axis.Y => _joystickTransform.rotation.eulerAngles.y,
                Axis.Z => _joystickTransform.rotation.eulerAngles.z,
                _ => 0
            };

            Power = new Vector3(joystickSideAngle, 0, joystickForwardAngle);
        }
    }

}