using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.CameraSystem;

namespace VSX.UniversalVehicleCombat.Mechs
{
    /// <summary>
    /// Mech controls.
    /// </summary>
    public class MCKPlayerInput_InputSystem_MechControls : VehicleInput
    {

        protected CharacterInputAsset mechInput;
        protected GeneralInputAsset generalInput;

        protected Vector2 look;
        protected Vector2 movement;

        [Header("Settings")]

        [SerializeField]
        protected GimballedVehicleController mechGimbalController;

        [SerializeField]
        protected RigidbodyCharacterController mechCharacterController;


        [Tooltip("Whether the legs move independently of the torso (as opposed to following where the torso is going).")]
        [SerializeField]
        protected bool independentLegs = false;


        [Tooltip("How much smoothness is applied to the rotation of the vehicle's gimbal when input is applied.")]
        [SerializeField]
        protected float rotationSmoothing;

        [Tooltip("The sensitivity of the look controls.")]
        [SerializeField]
        protected float lookSensitivity = 1f;

        [Tooltip("The sensitivity of the mouse look controls.")]
        [SerializeField]
        protected float mouseLookSensitivity = 0.05f;


        [Tooltip("The maximum angle from center that the mech can look horizontally.")]
        [SerializeField]
        protected float maxHorizontalLookRotation = 90;

        [Tooltip("The maximum angle from center that the mech can look vertically.")]
        [SerializeField]
        protected float maxVerticalLookRotation = 35;

        [Tooltip("Whether to invert the vertical input (e.g. when using a mouse).")]
        [SerializeField]
        protected bool invertVerticalInput = false;
        public bool InvertVerticalInput
        {
            get { return invertVerticalInput; }
            set { invertVerticalInput = value; }
        }

        // Store the previous frame's values so can smoothly lerp between previous and current values
        protected float lastHorizontalInputValue;
        protected float lastVerticalInputValue;

        [SerializeField]
        protected float movementSmoothing;

        protected Vector3 lastMovementInputs;

        public bool autoTarget;
        protected Weapons weapons;
        protected CameraTarget cameraTarget;

        




        protected override void Awake()
        {
            base.Awake();

            mechInput = new CharacterInputAsset();
            generalInput = new GeneralInputAsset();
            
            mechInput.CharacterControls.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();

            mechInput.CharacterControls.Run.performed += ctx => StartRunning();
            mechInput.CharacterControls.Run.canceled += ctx => StopRunning();

            mechInput.CharacterControls.Jump.performed += ctx => Jump();
            mechInput.CharacterControls.Jump.canceled += ctx => CancelJump();
        }


        protected virtual void OnEnable()
        {
            mechInput.Enable();
            generalInput.Enable();
        }

        protected virtual void OnDisable()
        {
            mechInput.Disable();
            generalInput.Disable();
        }


        private void OnValidate()
        {
            // Make sure rotation smoothing never falls below zero to prevent divide-by-zero error.
            rotationSmoothing = Mathf.Max(rotationSmoothing, 0);

            // Make sure smoothing is not negative to prevent divide-by-zero errors.
            movementSmoothing = Mathf.Max(movementSmoothing, 0);
        }

        // Called by the game agent that this input script belongs to when it enters a vehicle.
        protected override bool Initialize(Vehicle vehicle)
        {
            if (mechGimbalController != null)
            {
                mechGimbalController.IndependentRotationEnabled = false;
            }

            if (!base.Initialize(vehicle)) return false;

            mechGimbalController = vehicle.GetComponentInChildren<GimballedVehicleController>();
            mechCharacterController = vehicle.GetComponentInChildren<RigidbodyCharacterController>();
            weapons = vehicle.GetComponentInChildren<Weapons>();
            cameraTarget = vehicle.GetComponentInChildren<CameraTarget>();

            if (mechGimbalController != null && mechCharacterController != null)
            {
                mechGimbalController.IndependentRotationEnabled = true;
                return true;
            }
            else
            {
                mechGimbalController = null;
                mechCharacterController = null;
                return false;
            }
        }

        public override void DisableInput()
        {
            base.DisableInput();

            if (mechGimbalController != null)
            {
                // Stop moving and rotating the character
                mechCharacterController.SetMovementInputs(Vector3.zero);
                mechCharacterController.SetRotationInputs(Vector3.zero);
            }
        }


        protected virtual void StartRunning()
        {
            if (initialized) mechCharacterController.SetRunning(true);
        }


        protected virtual void StopRunning()
        {
            if (initialized) mechCharacterController.SetRunning(false);
        }


        protected virtual void Jump()
        {
            if (!initialized) return;

            if (mechCharacterController.Grounded)
            {
                mechCharacterController.Jump();
            }
            else
            {
                if (!mechCharacterController.Jetpacking)
                {
                    mechCharacterController.ActivateJetpack();
                }
            }
        }


        protected virtual void CancelJump()
        {
            if (!initialized) return;

            if (mechCharacterController.Jetpacking)
            {
                mechCharacterController.DeactivateJetpack();
            }
        }


        protected virtual void LegFollowerMovement()
        {
            // Get the target input vector
            Vector3 movementInputs = Vector3.ClampMagnitude(new Vector3(movement.x, 0f, movement.y), 1);

            // Get the next input vector
            movementInputs = Vector3.Lerp(lastMovementInputs, movementInputs, (1 / (1 + movementSmoothing)));
            lastMovementInputs = movementInputs;

            // Update whether the character is reversing
            bool reversing = movementInputs.z < -0.01f;
            mechCharacterController.SetReversing(reversing);

            // Convert the input to a local movement vector
            Vector3 worldMovementDirection = mechGimbalController.GimbalController.HorizontalPivot.TransformDirection(movementInputs).normalized;

            Vector3 localMovementDirection = mechCharacterController.transform.InverseTransformDirection(worldMovementDirection);

            // Send movement inputs
            mechCharacterController.SetMovementInputs(localMovementDirection * movementInputs.magnitude);

            mechGimbalController.IndependentRotationEnabled = true;

            // Rotate the character to face the movement vector
            float turnAmount = 0;
            if (movementInputs.magnitude > 0.01f)
            {
                Vector3 localLegsTargetDirection = localMovementDirection;

                if (reversing)
                {
                    localLegsTargetDirection *= -1;
                }

                turnAmount = Mathf.Atan2(localLegsTargetDirection.x, localLegsTargetDirection.z) * 10;
            }

            // Send rotation inputs to the character
            mechCharacterController.SetRotationInputs(new Vector3(0f, turnAmount, 0f));
        }



        protected virtual void LegIndependentMovement()
        {
            // Get the target input vector
            Vector3 movementInputs = Vector3.ClampMagnitude(new Vector3(0f, 0f, movement.y), 1);

            // Get the next input vector
            movementInputs = Vector3.Lerp(lastMovementInputs, movementInputs, (1 / (1 + movementSmoothing)));
            lastMovementInputs = movementInputs;

            mechCharacterController.SetReversing(movementInputs.z < 0);

            // Send movement inputs
            mechCharacterController.SetMovementInputs(movementInputs * movementInputs.magnitude);

            mechGimbalController.IndependentRotationEnabled = false;

            // Send rotation inputs to the character
            mechCharacterController.SetRotationInputs(new Vector3(0f, movement.x));

            mechGimbalController.GimbalController.NoConstraintsHorizontal = false;
            mechGimbalController.GimbalController.MinHorizontalPivotAngle = -maxHorizontalLookRotation;
            mechGimbalController.GimbalController.MaxHorizontalPivotAngle = maxHorizontalLookRotation;

            mechGimbalController.GimbalController.MinVerticalPivotAngle = -maxVerticalLookRotation;
            mechGimbalController.GimbalController.MaxVerticalPivotAngle = maxVerticalLookRotation;
        }


        // Called every frame that this input script is active
        protected override void InputUpdate()
        {
            if (autoTarget && weapons.WeaponsTargetSelector != null && weapons.WeaponsTargetSelector.SelectedTarget != null)
            {
                Vector3 pos = weapons.GetAverageLeadTargetPosition(weapons.WeaponsTargetSelector.SelectedTarget.WorldBoundsCenter, weapons.WeaponsTargetSelector.SelectedTarget.Velocity);
                if (cameraTarget != null && cameraTarget.CameraEntity != null)
                {
                    pos += mechGimbalController.GimbalController.VerticalPivot.position - cameraTarget.CameraEntity.transform.position;
                }

                mechGimbalController.TrackPosition(pos);
                
            }
            else
            {
                // Looking

                // Get the next horizontal and vertical inputs for the gimbal
                Vector2 lookInput = generalInput.GeneralControls.MouseDelta.ReadValue<Vector2>() * lookSensitivity * mouseLookSensitivity;
                Vector2 lookInput2 = mechInput.CharacterControls.Look.ReadValue<Vector2>() * lookSensitivity;

                if (Mathf.Abs(lookInput2.x) > Mathf.Abs(lookInput.x))
                {
                    lookInput.x = lookInput2.x;
                }
                if (Mathf.Abs(lookInput2.y) > Mathf.Abs(lookInput.y))
                {
                    lookInput.y = lookInput2.y;
                }

                float horizontalInputValue = Mathf.Lerp(lastHorizontalInputValue, lookInput.x, 1 / (1 + rotationSmoothing));
                float verticalInputValue = Mathf.Lerp(lastVerticalInputValue, lookInput.y, 1 / (1 + rotationSmoothing));

                // Rotate the gimbal
                mechGimbalController.SetRotationInputs(horizontalInputValue, -verticalInputValue * (invertVerticalInput ? -1 : 1));

                // Update the stored values for the next frame
                lastHorizontalInputValue = horizontalInputValue;
                lastVerticalInputValue = verticalInputValue;
            }

            if (independentLegs)
            {
                LegIndependentMovement();
            }
            else
            {
                LegFollowerMovement();
            }
        }
    }
}

