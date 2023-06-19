using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace VSX.UniversalVehicleCombat
{

    /// <summary>
    /// UnityEvent to run functions when a damageable is damaged.
    /// </summary>
    [System.Serializable]
    public class OnDamageableDamagedEventHandler : UnityEvent<float, Vector3, HealthModifierType, Transform> { }

    /// <summary>
    /// UnityEvent to run functions when a damageable is healed.
    /// </summary>
    [System.Serializable]
    public class OnDamageableHealedEventHandler : UnityEvent<float, Vector3, HealthModifierType, Transform> { }

    /// <summary>
    /// UnityEvent to run functions when a damageable is destroyed.
    /// </summary>
    [System.Serializable]
    public class OnDamageableDestroyedEventHandler : UnityEvent { }

    /// <summary>
    /// UnityEvent to run functions when a damageable is restored after being destroyed.
    /// </summary>
    [System.Serializable]
    public class OnDamageableRestoredEventHandler : UnityEvent { }


    [System.Serializable]
    public class CollisionEventHandler : UnityEvent<Collision> { }





    /// <summary>
    /// Makes an object damageable and healable.
    /// </summary>
    public class Damageable : MonoBehaviour
    {

        [Header("General")]

        [SerializeField]
        protected string damageableID;
        public string DamageableID { get { return damageableID; } }

        // The health type of this damageable
        [SerializeField]
        protected HealthType healthType;
        public HealthType HealthType { get { return healthType; } }

        // The maximum health value for the container
        [SerializeField]
        protected float healthCapacity = 100;
        public virtual float HealthCapacity
        {
            get { return healthCapacity; }
            set
            {
                healthCapacity = value;
                healthCapacity = Mathf.Max(healthCapacity, 0);
                currentHealth = Mathf.Min(currentHealth, healthCapacity);
            }
        }

        // The health value of the container when the scene starts
        [SerializeField]
        protected float startingHealth = 100;
        public virtual float StartingHealth { get { return startingHealth; } }

        // The current health value of the container
        protected float currentHealth;
        public virtual float CurrentHealth { get { return currentHealth; } }
        public virtual float CurrentHealthFraction { get { return currentHealth / startingHealth; } }

        public virtual void SetHealth(float newHealthValue)
        {
            currentHealth = Mathf.Clamp(newHealthValue, 0, healthCapacity);
        }

        // Enable/disable damage
        [SerializeField]
        protected bool isDamageable = true;

        [SerializeField]
        protected bool disableGameObjectOnDestroyed = true;

        // Enable/disable healing
        [SerializeField]
        protected bool isHealable = true;

        [SerializeField]
        protected bool canHealAfterDestroyed = false;


        [Header("Collisions")]

        [Tooltip("The coefficient multiplied by the collision relative velocity magnitude to get the damage value.")]
        [SerializeField]
        protected float collisionRelativeVelocityToDamageFactor = 2.5f;

        [SerializeField]
        protected HealthModifierType collisionHealthModifierType;

        [Tooltip("The maximum number of collision contacts allowed per collision. Damage is dealt for each contact point in a collision.")]
        [SerializeField]
        protected int collisionContactsLimit = 1;


        [Header("Events")]

        // Collision event
        public CollisionEventHandler onCollision;

        // Damageable damaged event
        public OnDamageableDamagedEventHandler onDamaged;

        // Damageable healed event
        public OnDamageableHealedEventHandler onHealed;

        // Damageable destroyed event
        public OnDamageableDestroyedEventHandler onDestroyed;

        // Damageable restored event
        public OnDamageableRestoredEventHandler onRestored;

        // Whether this damageable is currently destroyed
        protected bool destroyed = false;
        public bool Destroyed { get { return destroyed; } }



        /// <summary>
        /// Restore when object is enabled.
        /// </summary>
        protected virtual void OnEnable()
        {
            Restore(true);
        }

        /// <summary>
        /// Toggle whether this damageable is damageable.
        /// </summary>
        /// <param name="damageable">Whether this damageable is to be damageable.</param>
        public virtual void SetDamageable(bool isDamageable)
        {
            this.isDamageable = isDamageable;
        }


        /// <summary>
        /// Toggle whether this damageable is healable.
        /// </summary>
        /// <param name="healable">Whether this damageable is to be healable.</param>
        public void SetHealable(bool healable)
        {
            this.isHealable = healable;
        }


        /// <summary>
        /// Called when a collision happens to check if it involves a one of this damageable's colliders (if so, damages it).
        /// </summary>
        /// <param name="collision">The collision information.</param>
        public virtual void OnCollision(Collision collision)
        {
            for (int i = 0; i < collision.contacts.Length; ++i)
            {
                if (i == collisionContactsLimit) { break; }
                
                Damage(collision.relativeVelocity.magnitude * collisionRelativeVelocityToDamageFactor, collision.contacts[i].point, collisionHealthModifierType, null);
            }          
        }


        /// <summary>
        /// Damage this damageable.
        /// </summary>
        /// <param name="damage">The damage amount.</param>
        public virtual void Damage(float damage)
        {
            Damage(damage, transform.position);
        }


        /// <summary>
        /// Damage this damageable.
        /// </summary>
        /// <param name="damage">The damage amount.</param>
        /// <param name="hitPoint">The world position where the damage occurred.</param>
        /// <param name="healthModifierType">The type of health modification that occurred (can be used to drive effects etc).</param>
        /// <param name="damageSourceRootTransform">The root transform of the source of the damage (can be used to identify the source).</param>
        public virtual void Damage(float damage, Vector3 hitPoint, HealthModifierType healthModifierType = null, Transform damageSourceRootTransform = null)
        {

            if (destroyed) return;

            if (isDamageable)
            {
                // Reduce the health
                currentHealth = Mathf.Clamp(currentHealth - damage, 0, healthCapacity);

                // Call the damage event
                onDamaged.Invoke(damage, hitPoint, healthModifierType, damageSourceRootTransform);

                // Destroy
                if (Mathf.Approximately(currentHealth, 0))
                {
                    Destroy();
                } 
            }
        }

        
        /// <summary>
        /// Heal this damageable.
        /// </summary>
        /// <param name="healing">The healing amount.</param>
        public virtual void Heal(float healing)
        {
            Heal(healing, transform.position);
        }


        /// <summary>
        /// Heal this damageable.
        /// </summary>
        /// <param name="healing">The healing amount.</param>
        /// <param name="hitPoint">The world position where the healing occurred.</param>
        /// <param name="healthModifierType">The type of health modification that occurred (can be used to drive effects etc).</param>
        /// <param name="damageSourceRootTransform">The root transform of the source of the healing (can be used to identify the source).</param>
        public virtual void Heal(float healing, Vector3 hitPoint, HealthModifierType healthModifierType = null, Transform damageSourceRootTransform = null)
        {

            if (destroyed)
            {
                if (isHealable && canHealAfterDestroyed && healing > 0)
                {
                    Restore(false);
                }
                else
                {
                    return;
                }
            }

            if (isHealable)
            {
                // Add the health
                currentHealth = Mathf.Clamp(currentHealth + healing, 0, healthCapacity);

                onHealed.Invoke(healing, hitPoint, healthModifierType, damageSourceRootTransform);
            }
        }


        /// <summary>
        /// Destroy this damageable.
        /// </summary>
        public void Destroy()
        {
            // If already in the correct state, return
            if (destroyed) return;

            destroyed = true;

            // Call the destroyed event
            onDestroyed.Invoke();

            if (disableGameObjectOnDestroyed) gameObject.SetActive(false);

        }

        /// <summary>
        /// Restore this damageable.
        /// </summary>
        /// <param name="reset">Whether to reset to starting conditions.</param>
        public void Restore(bool reset = true)
        {

            destroyed = false;

            gameObject.SetActive(true);

            if (reset)
            {
                currentHealth = healthCapacity;
            }
            
            // Call the event
            onRestored.Invoke();
            
        }


        public virtual void SetColliderActivation(bool activate) { }
    }
}