using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;
using System.Linq;

/*
namespace VSX.UniversalVehicleCombat.Mechs
{
    public class MechInputSimple : MechInput
    {
        [SerializeField]
        protected MechController mechController;

        [SerializeField]
        protected float lookRotationSpeed = 60;
        public float LookRotationSpeed
        {
            get { return lookRotationSpeed; }
            set { lookRotationSpeed = value; }
        }

        [SerializeField]
        protected bool invertMouseVertical = false;
        public bool InvertMouseVertical
        {
            get { return invertMouseVertical; }
            set { invertMouseVertical = value; }
        }

        public float lookLerp = 0.5f;

        float lastMouseX;
        float lastMouseY;

        public float maxRotationSpeed = 1;

        public int numSmoothingValues;
        protected List<Vector2> smoothingValues;

        [HideInInspector]
        public Vector2 inputValues;

        public bool useLerp;
        public float lerpSpeed;

        public bool medianInput;

        protected Vector3 lastMovementInputs;


        public override void DisableInput()
        {
            base.DisableInput();
            if (mechController != null)
            {
                mechController.SetMovementInputs(Vector3.zero);
                mechController.SetRotationInputs(Vector3.zero);
            }

        }

        protected override void Start()
        {
            base.Start();

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                lookRotationSpeed = 250;
            }
        }

        protected override bool Initialize(Vehicle vehicle)
        {
            if (!base.Initialize(vehicle)) return false;

            mechController = vehicle.GetComponent<MechController>();

            return (mechController != null);
        }


        protected override void Awake()
        {

            base.Awake();

            smoothingValues = new List<Vector2>();
            for (int i = 0; i < numSmoothingValues; ++i)
            {
                smoothingValues.Add(Vector2.zero);
            }
        }

        protected override void InputUpdate()
        {

            UpdateSmoothingValues();

            // Get the movement input
            float forward = Input.GetAxis("Vertical");
            float right = Input.GetAxis("Horizontal");

            // Get the jump input
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (mechController.Grounded)
                {
                    mechController.Jump();
                }
                else
                {
                    mechController.ActivateJetpack();
                }
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                mechController.DeactivateJetpack();
            }

            float mouseX = (Input.GetAxis("Mouse X") / Screen.width) * 1000;
            float mouseY = (Input.GetAxis("Mouse Y") / Screen.width) * 1000;

            float valX = Mathf.Lerp(lastMouseX, lookRotationSpeed * mouseX * Time.deltaTime, lookLerp);
            float valY = Mathf.Lerp(lastMouseY, lookRotationSpeed * mouseY * Time.deltaTime, lookLerp);

            mechController.LookRotationAmounts(new Vector3(valX, valY * (invertMouseVertical ? -1 : 1), 0));

            // Rotate the mech according to the look controller
            Vector3 movementInputs = new Vector3(right, 0f, forward);

            mechController.SetRunning(Input.GetKey(KeyCode.LeftShift));

            movementInputs = Vector3.Lerp(lastMovementInputs, movementInputs, lerpSpeed * Time.deltaTime);
            lastMovementInputs = movementInputs;

            bool reversing = movementInputs.z < -0.01f;
            mechController.SetReversing(reversing);

            Vector3 worldMovementDirection = mechController.lookController.HorizontalPivot.TransformDirection(movementInputs).normalized;


            Vector3 localMovementDirection = mechController.transform.InverseTransformDirection(worldMovementDirection);
            mechController.SetMovementInputs(localMovementDirection * movementInputs.magnitude);

            float turnAmount = 0;
            if (movementInputs.magnitude > 0.01f)
            {
                Vector3 localLegsTargetDirection = localMovementDirection;
                if (reversing)
                {
                    localLegsTargetDirection *= -1;
                }

                Debug.DrawLine(mechController.transform.position, mechController.transform.position + localLegsTargetDirection * 50, Color.red);


                turnAmount = Mathf.Atan2(localLegsTargetDirection.x, localLegsTargetDirection.z) * 10;
            }

            mechController.SetRotationInputs(new Vector3(0f, turnAmount, 0f));

            lastMouseX = valX;
            lastMouseY = valY;
        }


        void UpdateSmoothingValues()
        {

            Vector2 nextValues = new Vector2(Input.GetAxis("Mouse X") / Time.deltaTime, Input.GetAxis("Mouse Y") / Time.deltaTime);

            if (useLerp)
            {
                inputValues = Vector2.Lerp(inputValues, nextValues, lerpSpeed);
            }
            else
            {
                if (numSmoothingValues == 0)
                {
                    inputValues = Vector2.Lerp(inputValues, nextValues, lerpSpeed);
                }
                else
                {
                    for (int i = smoothingValues.Count - 1; i > 0; --i)
                    {
                        smoothingValues[i] = smoothingValues[i - 1];
                    }

                    smoothingValues[0] = nextValues;


                    if (medianInput)
                    {
                        List<float> exes = new List<float>();
                        List<float> wyes = new List<float>();
                        for (int i = 0; i < smoothingValues.Count; ++i)
                        {
                            exes.Add(smoothingValues[i].x);

                            wyes.Add(smoothingValues[i].y);

                        }
                        exes = new List<float>(exes.OrderBy(x => x));
                        wyes = new List<float>(wyes.OrderBy(x => x));

                        int index = (smoothingValues.Count / 2) + 1;
                        inputValues = Vector2.Lerp(inputValues, new Vector2(exes[index], wyes[index]), lerpSpeed);
                    }
                    else
                    {
                        Vector2 newInputValues = Vector2.zero;
                        for (int i = 0; i < smoothingValues.Count; ++i)
                        {
                            newInputValues += smoothingValues[i];
                        }

                        newInputValues /= smoothingValues.Count;

                        inputValues = Vector2.Lerp(inputValues, newInputValues, lerpSpeed);
                    }
                }
            }

            rotationInputValues = new Vector2(Mathf.Clamp(inputValues.x / maxRotationSpeed, -1, 1),
                                                Mathf.Clamp(inputValues.y / maxRotationSpeed, -1, 1));
        }
    }
}
*/
