using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat.Radar
{
    public class TrackableModuleLinker : ModuleManager
    {
        protected override void OnModuleMounted(Module module)
        {
            base.OnModuleMounted(module);

            Trackable moduleTrackable = module.GetComponent<Trackable>();
            Trackable thisTrackable = GetComponent<Trackable>();
            if (thisTrackable != null && moduleTrackable != null)
            {
                thisTrackable.AddChildTrackable(moduleTrackable);
            }
        }
    }
}

