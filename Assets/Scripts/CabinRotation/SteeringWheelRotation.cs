﻿using System;
using UnityEngine;

namespace CabinRotation
{
    public class SteeringWheelRotation : CabinRotationBase
    {
        [SerializeField] private Transform _joystickTransform;
        [SerializeField] private Axis _controlAxis;
        [SerializeField] private float _deltaAngle;
        [SerializeField] private float _maxAngle;
        
        private void Update()
        {
            float joystickAngle = _controlAxis switch
            {
                Axis.X => _joystickTransform.localRotation.eulerAngles.x,
                Axis.Y => _joystickTransform.localRotation.eulerAngles.y,
                Axis.Z => _joystickTransform.localRotation.eulerAngles.z,
                _ => 0
            };

            Angle = Math.Abs(joystickAngle) > _deltaAngle ? joystickAngle / _maxAngle : 0;
        }

        public void GrabFinished()
        {
            _joystickTransform.localRotation = Quaternion.identity;
        }
    }
}