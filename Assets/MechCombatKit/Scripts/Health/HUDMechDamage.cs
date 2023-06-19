using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;
using VSX.UniversalVehicleCombat.Radar;
using UnityEngine.UI;

public class HUDMechDamage : MonoBehaviour
{
    [SerializeField]
    protected Health startingTargetHealth;

    protected Health targetHealth;

    [SerializeField]
    protected bool defineDisplayableTypes = false;

    [SerializeField]
    protected List<TrackableType> displayableTypes = new List<TrackableType>();

    [SerializeField]
    protected GameObject visualElementsParent;

    [SerializeField]
    protected List<HUDDisplayedDamageable> damageableDisplayItems = new List<HUDDisplayedDamageable>();

    
   

    private void Start()
    {
        if (startingTargetHealth != null)
        {
            SetTarget(startingTargetHealth);
        }
    }

    public void SetTarget(Trackable target)
    {
        if (target == null)
        {
            SetTarget((Health)null);
        }
        else
        {
            bool show = true;
            if (defineDisplayableTypes)
            {
                show = false;
                for (int i = 0; i < displayableTypes.Count; ++i)
                {
                    if (displayableTypes[i] == target.TrackableType)
                    {
                        show = true;
                        break;
                    }
                }
            }

            if (show)
            {
                Health health = target.GetComponent<Health>();
                SetTarget(health);
            }
        }
    }

    public void SetTarget(Health targetHealth)
    {

        for(int i = 0; i < damageableDisplayItems.Count; ++i)
        {
            damageableDisplayItems[i].Disconnect();
        }

        this.targetHealth = targetHealth;

        if (targetHealth == null)
        {
            visualElementsParent.SetActive(false);
            return;
        }

        visualElementsParent.SetActive(true);

        for (int i = 0; i < targetHealth.Damageables.Count; ++i)
        {
            for(int j = 0; j < damageableDisplayItems.Count; ++j)
            {
                if (damageableDisplayItems[j].DamageableID == targetHealth.Damageables[i].DamageableID)
                {
                    damageableDisplayItems[j].Connect(targetHealth.Damageables[i]);
                }
            }
        }
    }
}
