using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Controls a rotary weapon such as a gatling gun.
    /// </summary>
    public class RotatingWeapon : MonoBehaviour
    {
        [SerializeField] protected Transform rotatingTransform;
        [SerializeField] protected float rotationSpeed = -700;
        [SerializeField] protected float accelerationTime = 1;
        [SerializeField] protected float decelerationTime = 1;

        protected bool rotating = false;
        protected float stateChangeTime = -1000;      

        [SerializeField] protected Triggerable triggerable;

        protected bool canFire = false;
        public bool CanFire { get { return canFire; } }

        [SerializeField] protected AudioSource rotationAudio;
        [SerializeField] protected float maxRotationAudioVolume = 0.33f;
        [SerializeField] protected float startRotationAudioPitch = 1.33f;
        [SerializeField] protected float fullSpeedRotationAudioPitch = 2.2f;


        protected virtual void Reset()
        {
            rotatingTransform = transform;
        }


        protected virtual void Awake()
        {
            triggerable.onStartTriggering.AddListener(StartRotating);
            triggerable.onStopTriggering.AddListener(StopRotating);

            rotationAudio.volume = 0;
        }


        protected virtual void StartRotating()
        {
            rotating = true;
            stateChangeTime = Time.time;
        }


        protected virtual void StopRotating()
        {
            rotating = false;
            stateChangeTime = Time.time;
        }


        protected virtual void Update()
        {
            if (rotating)
            {
                float accelerationMultiplier = Mathf.Min((Time.time - stateChangeTime) / accelerationTime, 1);
                rotatingTransform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime * accelerationMultiplier));

                if (rotationAudio != null)
                {
                    rotationAudio.pitch = accelerationMultiplier * fullSpeedRotationAudioPitch + (1 - accelerationMultiplier) * startRotationAudioPitch;
                    rotationAudio.volume = maxRotationAudioVolume * accelerationMultiplier;
                }
            }
            else
            {
                float decelerationMultiplier = (Time.time - stateChangeTime) / decelerationTime;
                if (decelerationMultiplier < 1)
                {
                    rotatingTransform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime * (1 - decelerationMultiplier)));

                    if (rotationAudio != null)
                    {
                        rotationAudio.pitch = decelerationMultiplier * startRotationAudioPitch + (1 - decelerationMultiplier) * fullSpeedRotationAudioPitch;
                        rotationAudio.volume = maxRotationAudioVolume * (1 - decelerationMultiplier);
                    }
                }
                else
                {
                    if (rotationAudio != null)
                    {
                        rotationAudio.volume = 0;
                    }
                }
            }

            canFire = rotating && (Time.time - stateChangeTime > accelerationTime);

        }
    }
}
