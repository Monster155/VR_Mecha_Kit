using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;
using VSX.UniversalVehicleCombat.Radar;
using UnityEngine.Events;

[System.Serializable]
public class DamageableMovementModifier
{
    public Damageable damageable;
    public float movementModifier = 0.5f;
}

public class MechDestructionManager : MonoBehaviour
{

    [Header("Settings")]

    [SerializeField]
    protected Vehicle vehicle;

    [SerializeField]
    protected RigidbodyCharacterController characterController;

    [SerializeField]
    protected GimballedVehicleController mechGimbal;

    [SerializeField]
    protected Animator m_Animator;

    [SerializeField]
    protected Trackable trackable;

    [SerializeField]
    protected CapsuleCollider capsuleCollider;
    
    [SerializeField]
    protected List<Collider> bodyColliders = new List<Collider>();

    [Header("Damage")]

    [SerializeField]
    protected List<DamageableMovementModifier> damageableMovementModifiers = new List<DamageableMovementModifier>();

    protected float currentMovementModifier = 1;

    [Header("Death")]

    [SerializeField]
    protected List<Collider> deathEnabledColliders = new List<Collider>();

    [SerializeField]
    protected List<Collider> deathDisabledColliders = new List<Collider>();

    [SerializeField]
    protected List<MonoBehaviour> deathDisabledComponents = new List<MonoBehaviour>();

    [SerializeField]
    protected float deathGravity = 4;
    
    protected bool destroyed = false;

    public Vector3 deathRigidbodyTorque = new Vector3(4000, 0, 0);
    public Vector3 deathRigidbodyForce = new Vector3(0, 100, 0);

    protected List<Damageable> damageables = new List<Damageable>();


    private void Awake()
    {
        foreach(DamageableMovementModifier damageableMovementModifier in damageableMovementModifiers)
        {
            damageableMovementModifier.damageable.onDestroyed.AddListener(OnDamageableMovementModifierDestroyed);
        }

        vehicle.onDestroyed.AddListener(OnDestroyed);
        vehicle.onRestored.AddListener(OnRestored);

        damageables = new List<Damageable>(GetComponentsInChildren<Damageable>());
    }
    
    protected void Reset()
    {

        vehicle = GetComponent<Vehicle>();
        characterController = GetComponent<RigidbodyCharacterController>();
        m_Animator = GetComponent<Animator>();
        trackable = GetComponent<Trackable>();
        m_Animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        Collider[] collidersArray = transform.GetComponentsInChildren<Collider>();
        foreach(Collider collider in collidersArray)
        {
            bodyColliders.Add(collider);
        }
    }


    protected void OnDamageableMovementModifierDestroyed()
    {
        currentMovementModifier = 1;

        for(int i = 0; i < damageableMovementModifiers.Count; ++i)
        {
            if (damageableMovementModifiers[i].damageable.Destroyed)
            {
                currentMovementModifier -= damageableMovementModifiers[i].movementModifier;
            }
        }

        characterController.SetMovementModifier(currentMovementModifier);

    }

    public void OnDestroyed()
    {

        characterController.ClearInputs();
        characterController.CharacterControllerEnabled = false;
        mechGimbal.SetRotationInputs(0, 0);
        mechGimbal.enabled = false;

        if (trackable != null)
        {
            trackable.SetActivation(false);
        }

        capsuleCollider.enabled = false;
        m_Animator.enabled = false;
        
        for (int i = 0; i < deathDisabledComponents.Count; ++i)
        {
            deathDisabledComponents[i].enabled = false;
        }

        // Free the rigidbody to fall over
        vehicle.CachedRigidbody.useGravity = true;
        vehicle.CachedRigidbody.constraints = RigidbodyConstraints.None;
        vehicle.CachedRigidbody.mass = 20;
        vehicle.CachedRigidbody.angularDrag = 2f;

        // Make the colliders collideable
        for (int i = 0; i < bodyColliders.Count; ++i)
        {
            bodyColliders[i].isTrigger = false;
        }

        for (int i = 0; i < deathDisabledColliders.Count; ++i)
        {
            deathDisabledColliders[i].enabled = false;
        }

        for (int i = 0; i < deathEnabledColliders.Count; ++i)
        {
            deathEnabledColliders[i].enabled = true;
        }

        vehicle.CachedRigidbody.AddTorque(deathRigidbodyTorque);
        vehicle.CachedRigidbody.AddForce(deathRigidbodyForce);

        foreach(Damageable damageable in damageables)
        {
            damageable.Destroy();
        }

        destroyed = true;
    }

    protected virtual void OnRestored()
    {
        characterController.CharacterControllerEnabled = true;
        mechGimbal.enabled = true;
    }

    private void FixedUpdate()
    {
        if (destroyed)
        {
            vehicle.CachedRigidbody.AddForce(-Vector3.up * deathGravity);
        }
    }
}
