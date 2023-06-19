using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;
using UnityEngine.Events;

namespace VSX.UniversalVehicleCombat
{

    [System.Serializable]
    public class FallAffectedDamageable
    {
        public Damageable damageable;
        public AnimationCurve landingVelocityToDamageCurve = AnimationCurve.Linear(0, 0, 1, 1);

        public float maxFallDamage;
        public float minFallDamageVelocity = 5;
        public float maxFallDamageVelocity = 20;
    }


    public class RigidbodyCharacterFallDamage : MonoBehaviour
    {

        [SerializeField]
        protected RigidbodyCharacterController characterController;

        [SerializeField]
        protected List<FallAffectedDamageable> fallAffectedDamageables = new List<FallAffectedDamageable>();

        [SerializeField]
        protected Animator animatorController;

        [SerializeField]
        protected string fallDamageAnimatorKey = "LandDamageAmount";

        [SerializeField]
        protected AudioSource landDamageAudio;


        protected virtual void Reset()
        {
            characterController = GetComponent<RigidbodyCharacterController>();
            animatorController = GetComponent<Animator>();
        }

        protected virtual void Awake()
        {
            characterController.onGrounded.AddListener(OnGrounded);
        }

        protected virtual void OnGrounded(Vector3 landingVelocity)
        {
            for (int i = 0; i < fallAffectedDamageables.Count; ++i)
            {
                float curveValue = Mathf.Clamp((-landingVelocity.y - fallAffectedDamageables[i].minFallDamageVelocity) /
                                                (fallAffectedDamageables[i].maxFallDamageVelocity - fallAffectedDamageables[i].minFallDamageVelocity), 0, 1);

                float damageValue = fallAffectedDamageables[i].landingVelocityToDamageCurve.Evaluate(curveValue) * fallAffectedDamageables[i].maxFallDamage;

                if (damageValue > 0.001f)
                {
                    fallAffectedDamageables[i].damageable.Damage(damageValue, fallAffectedDamageables[i].damageable.transform.position, null, null);
                }

                if (animatorController != null && fallDamageAnimatorKey != "")
                {
                    animatorController.SetFloat(fallDamageAnimatorKey, curveValue);
                }

                if (landDamageAudio != null)
                {
                    landDamageAudio.volume = 1 * curveValue;
                    landDamageAudio.Play();
                }
            }
        }
    }
}
