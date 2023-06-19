using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat.Mechs
{
    public class RigidbodyCharacterJetpackAudio : MonoBehaviour
    {
        [SerializeField]
        protected RigidbodyCharacterController m_RigidbodyCharacterController;

        [SerializeField]
        protected AudioSource jetpackAudio;

        [SerializeField]
        protected float maxVolume = 1;

        [SerializeField]
        protected float volumeChangeSpeed = 5;


        private void Awake()
        {
            jetpackAudio.loop = true;
            jetpackAudio.volume = 0;
            jetpackAudio.Play();
        }

        protected void Update()
        {
            if (m_RigidbodyCharacterController.Jetpacking)
            {
                jetpackAudio.volume = Mathf.Lerp(jetpackAudio.volume, maxVolume, volumeChangeSpeed * Time.deltaTime);
            }
            else
            {
                jetpackAudio.volume = Mathf.Lerp(jetpackAudio.volume, 0, volumeChangeSpeed * Time.deltaTime);
            }
        }
    }
}
