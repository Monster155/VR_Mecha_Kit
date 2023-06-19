using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VSX.UniversalVehicleCombat.Mechs
{
    public class MCK_ControlsPopupController : MonoBehaviour
    {
        [SerializeField]
        protected GameObject controlsPanel;

        [SerializeField]
        protected TextMeshProUGUI label;

        private void Awake()
        {
            controlsPanel.SetActive(false);
        }

        public void Toggle()
        {
            controlsPanel.SetActive(!controlsPanel.activeSelf);

            if (controlsPanel.activeSelf)
            {
                label.text = "PRESS C TO HIDE CONTROLS";
            }
            else
            {
                label.text = "PRESS C FOR CONTROLS";
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Toggle();
            }
        }
    }
}
