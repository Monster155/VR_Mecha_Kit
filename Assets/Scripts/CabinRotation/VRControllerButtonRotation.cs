using CabinRotation;
using UnityEngine;

namespace Movement
{
    public class VRControllerButtonRotation : CabinRotationBase
    {
        public void OnPrimaryButtonPressed()
        {
            Angle = 10;
        }

        public void OnSecondaryButtonPressed()
        {
            Angle = -10;
        }

        public void OnButtonRelease()
        {
            Angle = 0;
        }
    }
}