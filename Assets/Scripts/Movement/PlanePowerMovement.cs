using System;
using CabinRotation;
using UnityEngine;

namespace Movement
{
    public class PlanePowerMovement : LegsMovementBase
    {
        [SerializeField] private Transform _joystickTransform;
        [SerializeField] private Axis _rotationAxis;
        [SerializeField] private float _deltaAngle;
        [SerializeField] private float _maxAngle;

        private void Update()
        {
            float joystickAngle = _rotationAxis switch
            {
                Axis.X => _joystickTransform.localRotation.eulerAngles.x,
                Axis.Y => _joystickTransform.localRotation.eulerAngles.y,
                Axis.Z => _joystickTransform.localRotation.eulerAngles.z,
                _ => 0
            };

            Power = new Vector3(0, 0, Math.Abs(joystickAngle) > _deltaAngle ? joystickAngle / _maxAngle : 0);
        }

        public void GrabFinished()
        {
            _joystickTransform.localRotation = Quaternion.identity;
        }
    }
}