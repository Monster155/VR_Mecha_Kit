using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat.Mechs
{
    /// <summary>
    /// Example AI script for a mech that patrols and engages targets.
    /// </summary>
    public class AIController : MechInput
    {

        [Tooltip("How fast the AI will turn to look at something (e.g. a target).")]
        [SerializeField] protected float lookSpeed = 5;

        [Tooltip("The field of view in which the AI can see things (e.g. a target).")]
        [SerializeField] protected float fieldOfView = 180;


        [Header ("Patrol")]

        [Tooltip("The patrol route that the AI should follow.")]
        [SerializeField] protected PatrolRoute patrolRoute;

        protected int patrolWaypointIndex = 0;


        [Header("Combat")]

        [Tooltip("The combat zone that the AI will remain within during combat.")]
        [SerializeField]
        protected CombatZone combatZone;

        [Tooltip("How long the AI should stay in the attacking state during combat before alternating with evading.")]
        [SerializeField]
        protected float attackTime = 2.5f;

        [Tooltip("How long the AI should stay in the evading state during combat before alternating with attacking.")]
        [SerializeField]
        public float evadeTime = 5;

        protected Vector3 moveTargetPosition;   // The next position that the mech will move toward (combat or noncombat)

        // Combat parameters

        protected bool attacking = false;
        protected float nextCombatTime = 0;
        protected float combatStateStartTime = 0;
        protected bool isCombat = false;
        protected bool combatDelayed = false;

        // Mech components

        protected RigidbodyCharacterController mechCharacterController;

        protected GimballedVehicleController mechGimbalController;

        protected Weapons weapons;

        protected TriggerablesManager triggerablesManager;


        /// <summary>
        /// Called to disable the input
        /// </summary>
        public override void DisableInput()
        {
            base.DisableInput();

            // Clear ongoing rotation and movement
            if (mechCharacterController != null)
            {
                mechCharacterController.SetMovementInputs(Vector3.zero);
                mechCharacterController.SetRotationInputs(Vector3.zero);
            }

        }


        /// <summary>
        /// Set the vehicle for the AI.
        /// </summary>
        /// <param name="vehicle">The vehicle.</param>
        public override void SetVehicle(Vehicle vehicle)
        {
            if (triggerablesManager != null)
            {
                triggerablesManager.StopTriggeringAll();
            }

            base.SetVehicle(vehicle);

        }


        // Initialize this input with a vehicle
        protected override bool Initialize(Vehicle vehicle)
        {
            if (!base.Initialize(vehicle)) return false;

            mechCharacterController = vehicle.GetComponent<RigidbodyCharacterController>();
            mechGimbalController = vehicle.GetComponent<GimballedVehicleController>();

            triggerablesManager = vehicle.GetComponent<TriggerablesManager>();
            weapons = vehicle.GetComponent<Weapons>();

            if (mechCharacterController != null && mechGimbalController != null)
            {

                moveTargetPosition = vehicle.transform.position;

                Damageable[] damageables = vehicle.transform.GetComponentsInChildren<Damageable>();
                foreach (Damageable damageable in damageables)
                {
                    damageable.onDamaged.AddListener(delegate { OnHit(); });
                }

                return true;
            }
            else
            {
                return false;
            }
        }


        // Get whether the mech has arrived at its current target position
        protected virtual bool ReachedTargetPosition()
        {
            return (Vector3.Distance(new Vector3(moveTargetPosition.x, mechCharacterController.transform.position.y, moveTargetPosition.z), mechCharacterController.transform.position) < 2f);
        }


        // Look at a world position
        protected virtual void LookAtPosition(Vector3 pos)
        {
            Vector3 horizontalLocalPos = mechGimbalController.GimbalController.HorizontalPivot.InverseTransformPoint(pos);

            float lookH = Mathf.Atan2(horizontalLocalPos.x, horizontalLocalPos.z) * lookSpeed;

            Vector3 verticalLocalPos = mechGimbalController.GimbalController.VerticalPivot.InverseTransformPoint(pos);

            float lookV = -Mathf.Atan2(verticalLocalPos.y, verticalLocalPos.z) * lookSpeed;

            if (Mathf.Abs(lookH) > 0.5f)
            {
                lookV = 0;
            }

            mechGimbalController.SetRotationInputs(lookH, lookV);
        }


        // Get the next patrol waypoint
        protected virtual void IteratePatrolWaypoint()
        {
            if (patrolWaypointIndex == patrolRoute.Waypoints.Count - 1)
            {
                patrolWaypointIndex = 0;
            }
            else
            {
                patrolWaypointIndex++;
            }
        }


        // Update the patrol behaviour
        protected virtual void Patrol()
        {
            moveTargetPosition = patrolRoute.Waypoints[patrolWaypointIndex].position;

            Vector3 targetLookPos = moveTargetPosition;
            targetLookPos.y = mechGimbalController.GimbalController.HorizontalPivot.position.y;

            LookAtPosition(targetLookPos);

            if (triggerablesManager != null)
            {
                triggerablesManager.StopTriggeringAll();
            }

            if (ReachedTargetPosition())
            {
                IteratePatrolWaypoint();
            }

            isCombat = false;
        }


        // Whether the AI currently has a target
        protected virtual bool HasTarget()
        {
            if (weapons != null)
            {
                if (weapons.WeaponsTargetSelector != null)
                {
                    if (weapons.WeaponsTargetSelector.SelectedTarget != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        // Called when the combat behaviour begins
        protected virtual void OnCombatBegin()
        {
            attacking = true;
            nextCombatTime = attackTime;
            combatStateStartTime = Time.time;

            GetNewCombatMoveTarget();
        }


        // Update the combat behaviour
        protected virtual void Combat()
        {

            if (combatDelayed) return;

            if (!isCombat)
            {
                OnCombatBegin();
            }

            Vector3 targetLookPos;

            Vector3 targetPos = weapons.WeaponsTargetSelector.SelectedTarget.transform.TransformPoint(weapons.WeaponsTargetSelector.SelectedTarget.TrackingBounds.center);

            if (attacking)
            {
                targetLookPos = targetPos;
            }
            else
            {
                targetLookPos = targetPos;//mechController.GimbalController.HorizontalPivot.position + mechController.transform.forward;
            }

            LookAtPosition(targetLookPos);

            if (attacking && Vector3.Angle(mechGimbalController.GimbalController.VerticalPivot.forward, targetPos - mechCharacterController.transform.position) < 15)
            {
                if (triggerablesManager != null)
                {
                    if (!triggerablesManager.IsTriggering(0))
                    {
                        triggerablesManager.StartTriggeringAtIndex(0);
                    }
                }
            }
            else
            {
                triggerablesManager.StopTriggeringAll();
            }

            if (Time.time - combatStateStartTime > nextCombatTime)
            {
                attacking = !attacking;
                combatStateStartTime = Time.time;
                nextCombatTime = attacking ? attackTime : evadeTime;
            }

            if (ReachedTargetPosition())
            {
                GetNewCombatMoveTarget();
            }

            isCombat = true;

        }


        // Get a move target for the combat behaviour
        protected virtual void GetNewCombatMoveTarget()
        {
            Vector3 targetPos = weapons.WeaponsTargetSelector.SelectedTarget.transform.TransformPoint(weapons.WeaponsTargetSelector.SelectedTarget.TrackingBounds.center);

            Vector3 fromTarget = (mechCharacterController.transform.position - targetPos).normalized;
            moveTargetPosition = targetPos + fromTarget * 50 + (Quaternion.Euler(0f, 90f * (1 + Random.Range(-1, 1) * 2), 0f) * fromTarget) * 50;

            moveTargetPosition = combatZone.ClampToBounds(moveTargetPosition);
        }


        // Called when the AI mech is hit
        protected virtual void OnHit()
        {
            if (!isCombat && HasTarget() && !combatDelayed)
            {
                StartCoroutine(CombatDelay());
            }
        }


        // Create a small delay before combat (so the mech does not have instantaneous reactions
        protected virtual IEnumerator CombatDelay()
        {
            combatDelayed = true;
            yield return new WaitForSeconds(0.5f);
            combatDelayed = false;

            if (HasTarget())
            {
                OnCombatBegin();

                isCombat = true;
            }
        }


        // Input update
        protected override void InputUpdate()
        {
            if (HasTarget())
            {
                if (!isCombat)
                {

                    Vector3 targetPos = weapons.WeaponsTargetSelector.SelectedTarget.transform.TransformPoint(weapons.WeaponsTargetSelector.SelectedTarget.TrackingBounds.center);
                    if (!combatDelayed && Vector3.Angle(mechGimbalController.GimbalController.HorizontalPivot.forward, targetPos - 
                        mechGimbalController.GimbalController.HorizontalPivot.position) < (fieldOfView / 2))
                    {
                        StartCoroutine(CombatDelay());
                        Combat();
                    }
                    else
                    {
                        Patrol();
                    }
                }
                else
                {
                    Combat();
                }
            }
            else
            {
                Patrol();
            }


            moveTargetPosition.y = mechCharacterController.transform.position.y;

            Vector3 localLegsMovementDirection = mechCharacterController.transform.InverseTransformDirection(moveTargetPosition - mechCharacterController.transform.position).normalized;
            Vector3 localLookMovementDirection = mechGimbalController.GimbalController.HorizontalPivot.InverseTransformDirection(moveTargetPosition - mechCharacterController.transform.position).normalized;

            float turnAmount = 0;
            bool reversing = localLookMovementDirection.z < -0.01f;
            if (localLegsMovementDirection.magnitude > 0.01f)
            {
                Vector3 localLegsTargetDirection = localLegsMovementDirection;
                if (reversing)
                {
                    localLegsTargetDirection *= -1;
                }
                
                turnAmount = Mathf.Atan2(localLegsTargetDirection.x, localLegsTargetDirection.z) * 10;
                
            }

            mechCharacterController.SetRotationInputs(new Vector3(0f, turnAmount, 0));

            mechCharacterController.SetReversing(true);

            mechCharacterController.SetMovementInputs(localLegsMovementDirection.normalized);

            
        }
    }

}
