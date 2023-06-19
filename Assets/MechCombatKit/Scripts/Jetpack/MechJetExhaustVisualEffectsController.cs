using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;

public class MechJetExhaustVisualEffectsController : JetExhaustVisualEffectsController
{

    public RigidbodyCharacterController characterController;


    protected override void CalculateEffectsParameters()
    {
        targetCruisingEffectsAmount = characterController.Jetpacking ? 1 : 0;
        targetBoostEffectsAmount = 0;
    }
}
