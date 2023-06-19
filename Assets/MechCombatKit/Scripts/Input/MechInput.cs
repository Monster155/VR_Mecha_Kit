using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;

public class MechInput : VehicleInput
{

    protected float maxMovementInput;

    [SerializeField]
    protected Vector3 rotationInputValues;
    public Vector3 RotationInputValues { get { return rotationInputValues; } }

    [SerializeField]
    protected Vector3 movementInputValues;

    public void SetMaxMovementInput(float maxMovementInput)
    {
        this.maxMovementInput = maxMovementInput;
    }
}
