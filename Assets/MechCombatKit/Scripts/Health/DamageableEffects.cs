using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    [System.Serializable]
    public class DamageEnabledObject
    {
        public GameObject damageEnabledObject;
        public float healthFractionThreshold = 0.5f;
    }

    public class DamageableEffects : MonoBehaviour
    {

        [SerializeField]
        protected Damageable damageable;

        [SerializeField]
        protected List<DamageEnabledObject> damageEnabledObjects = new List<DamageEnabledObject>();


        protected virtual void Reset()
        {
            damageable = GetComponent<Damageable>();
        }

        private void Awake()
        {
            damageable.onDamaged.AddListener(delegate { OnDamaged(); });
        }

        protected virtual void OnDamaged()
        {
            for (int i = 0; i < damageEnabledObjects.Count; ++i)
            {
                if (!damageEnabledObjects[i].damageEnabledObject.activeSelf)
                {
                    if (damageable.CurrentHealthFraction < damageEnabledObjects[i].healthFractionThreshold)
                    {
                        damageEnabledObjects[i].damageEnabledObject.SetActive(true);
                    }
                }
            }
        }
    }
}
