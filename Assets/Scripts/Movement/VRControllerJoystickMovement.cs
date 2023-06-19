using UnityEngine;

namespace Movement
{
    public class VRControllerJoystickMovement : LegsMovementBase
    {
        public void OnXValueChanged(float x)
        {
            Power = new Vector3(x, Power.y, Power.z);
        }

        public void OnYValueChanged(float y)
        {
            Power = new Vector3(Power.x, y, Power.z);
        }
    }
}