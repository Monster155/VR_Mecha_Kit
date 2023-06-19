using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat.Mechs
{
    /// <summary>
    /// Manages the main demo for the Mech Combat Kit.
    /// </summary>
    public class MechDemo_DemoManager : MonoBehaviour
    {
        [SerializeField]
        protected GameAgent player;

        [SerializeField]
        protected string mainMenuSelectionKey = "MainMenuSelection";

        [Header("Mech Options")]

        [SerializeField]
        protected Vehicle mechSimple;

        [SerializeField]
        protected Vehicle mechComplex;

        [Header("Override")]

        [SerializeField]
        protected bool overrideIndex;

        [SerializeField]
        protected int overrideIndexValue = 0;


        private void Start()
        {
            int index = overrideIndex ? overrideIndexValue : PlayerPrefs.GetInt(mainMenuSelectionKey, 0);
            if (index == 0)
            {
                if (mechComplex != null) mechComplex.gameObject.SetActive(false);
                if (mechSimple != null)
                {
                    mechSimple.gameObject.SetActive(true);
                    player.EnterVehicle(mechSimple);
                }
            }
            else
            {
                if (mechSimple != null) mechSimple.gameObject.SetActive(false);
                if (mechComplex != null)
                {
                    mechComplex.gameObject.SetActive(true);
                    player.EnterVehicle(mechComplex);
                }
            }
        }
    }

}
