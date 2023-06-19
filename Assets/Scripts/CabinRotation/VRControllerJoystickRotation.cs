using System;
using UnityEngine;

namespace CabinRotation
{
    public class VRControllerJoystickRotation : CabinRotationBase
    {
        private void Update()
        {
            // _playerTransform.Rotate(_rotationScale * new Vector3(
            //     0,
            //     OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x,
            //     0));
            //
            // _playerTransform.Translate(_moveScale * new Vector3(
            //     OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x,
            //     0,
            //     OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y));
        }

        public void OnXValueChanged(float x)
        {
            Debug.LogError("X=" + x);
        }

        public void OnYValueChanged(float y)
        {
            Debug.LogError("Y=" + y);
        }
    }
}