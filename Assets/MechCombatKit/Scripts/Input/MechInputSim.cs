using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace VSX.UniversalVehicleCombat.Mechs
{
    public class MechInputSim : RigidbodyCharacterInput
    {

        protected override void InputUpdate()
        {
            base.InputUpdate();

            // Get the movement input
            float forward = Input.GetAxis("Vertical");
            float right = Input.GetAxis("Horizontal");

            // Get the target input vector
            Vector3 movementInputs = Vector3.ClampMagnitude(new Vector3(0f, 0f, forward), 1);

            // Get the next input vector
            movementInputs = Vector3.Lerp(lastMovementInputs, movementInputs, (1 / (1 + movementSmoothing)));
            lastMovementInputs = movementInputs;

            // Send movement inputs
            m_RigidbodyCharacterController.SetMovementInputs(movementInputs * movementInputs.magnitude);

            // Send rotation inputs to the character
            m_RigidbodyCharacterController.SetRotationInputs(new Vector3(0f, right, 0f));

        }
    }
}

