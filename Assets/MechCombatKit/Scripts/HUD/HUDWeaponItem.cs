using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;
using TMPro;
using VSX.Utilities.UI;

public class HUDWeaponItem : MonoBehaviour
{
    public UIFillBarController cooldownBar;
    public TextMeshProUGUI label;


    public void SetLabel(string labelValue)
    {
        label.text = labelValue;
    }

    public void SetCooldownBarValue(float value)
    {
        cooldownBar.SetFillAmount(value);
    }
}
