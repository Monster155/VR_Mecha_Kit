using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;
using TMPro;
using VSX.Pooling;

public class HUDWeaponDisplay : ModuleManager
{
    public HUDWeaponItem itemPrefab;

    public Transform itemParent;

    public List<ModuleType> displayedModuleTypes = new List<ModuleType>();

    protected List<DisplayedModule> displayedModules = new List<DisplayedModule>();

    public class DisplayedModule
    {
        public Module module;
        public CooldownTimer cooldownTimer;
        public HUDWeaponItem displayItem;

        public DisplayedModule(Module module, HUDWeaponItem displayItem)
        {
            this.module = module;
            this.displayItem = displayItem;
        }
    }


    protected override void OnModuleMounted(Module module)
    {
        base.OnModuleMounted(module);

        bool found = false;
        for(int i = 0; i < displayedModuleTypes.Count; ++i)
        {
            if (module.ModuleType == displayedModuleTypes[i])
            {
                found = true;
                break;
            }
        }

        if (found)
        {

            HUDWeaponItem item;

            if (PoolManager.Instance != null)
            {
                item = PoolManager.Instance.Get(itemPrefab.gameObject, Vector3.zero, Quaternion.identity, itemParent).GetComponent<HUDWeaponItem>();
            }
            else
            {
                item = Instantiate(itemPrefab.gameObject, Vector3.zero, Quaternion.identity, itemParent).GetComponent<HUDWeaponItem>();
            }
             

            item.transform.localRotation = Quaternion.identity;
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = new Vector3(1, 1, 1);

            DisplayedModule displayedModule = new DisplayedModule(module, item);

            item.SetLabel(module.Label);

            item.cooldownBar.gameObject.SetActive(false);
            CooldownTimer cooldownTimer = module.GetComponent<CooldownTimer>();
            if (cooldownTimer != null)
            {
                item.cooldownBar.gameObject.SetActive(true);
                cooldownTimer.onCooldownValueChanged.AddListener(item.cooldownBar.SetFillAmount);
            }

            displayedModules.Add(displayedModule);

            OrderItems();
        }
    }

    protected override void OnModuleUnmounted (Module module)
    {
        base.OnModuleUnmounted(module);

        for (int i = 0; i < displayedModules.Count; ++i)
        {
            if (displayedModules[i].module == module)
            {
                if (displayedModules[i].cooldownTimer != null)
                {
                    displayedModules[i].cooldownTimer.onCooldownValueChanged.RemoveListener(displayedModules[i].displayItem.cooldownBar.SetFillAmount);
                }

                displayedModules[i].displayItem.gameObject.SetActive(false);

                displayedModules.RemoveAt(i);
                break;
            }
        }
    }

    protected void OrderItems()
    {
        
        List<DisplayedModule> newDisplayedModules = new List<DisplayedModule>();

        for(int i = 0; i < displayedModuleTypes.Count; ++i)
        {
            for (int j = 0; j < displayedModules.Count; ++j)
            {
                if (displayedModules[j].module.ModuleType == displayedModuleTypes[i])
                {
                    newDisplayedModules.Add(displayedModules[j]);
                }
            }
        }

        displayedModules = newDisplayedModules;

        for (int i = 0; i < displayedModules.Count; ++i)
        {
            displayedModules[i].displayItem.transform.SetSiblingIndex(i);
        }   
    }
}
