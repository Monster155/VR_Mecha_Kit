using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VSX.UniversalVehicleCombat.Mechs
{
    // Manages the settings for the main demo
    public class MechDemo_SettingsControls : MonoBehaviour
    {
        [Header("General")]

        [SerializeField]
        protected GameObject settingsPanel;

        [SerializeField]
        protected TextMeshProUGUI buttonText;

        [SerializeField]
        protected Slider lookSensitivitySlider;

        [SerializeField]
        protected Toggle mouseInvertToggle;

        [Header("Input")]

        [SerializeField]
        protected GimballedVehicleControls mechGimbalControls;

        [SerializeField]
        protected GimballedVehicleController mechGimbalController;

        [SerializeField]
        protected Vector2 minMaxLookSpeed;


        private void Awake()
        {
            settingsPanel.SetActive(false);

            lookSensitivitySlider.onValueChanged.AddListener(SetLookSensitivity);

            mouseInvertToggle.isOn = false;
            mouseInvertToggle.onValueChanged.AddListener(ToggleInvertMouse);
        }

        /// <summary>
        /// Toggle the settings panel on/off 
        /// </summary>
        public void ToggleSettingsPanel()
        {
            if (settingsPanel.activeSelf)
            {
                settingsPanel.SetActive(false);
                buttonText.text = "PRESS P FOR SETTINGS";
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                settingsPanel.SetActive(true);
                buttonText.text = "PRESS P TO HIDE SETTINGS";
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void SetLookSensitivity(float sensitivity)
        {
            if (mechGimbalControls != null && mechGimbalControls.Initialized && mechGimbalControls.InputEnabled)
            {
                mechGimbalController.RotationSpeed = minMaxLookSpeed.x + sensitivity * (minMaxLookSpeed.y - minMaxLookSpeed.x);
            }
        }

        public void ToggleInvertMouse(bool inverted)
        {
            if (mechGimbalControls != null) mechGimbalControls.InvertVerticalInput = inverted;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                ToggleSettingsPanel();
            }

            if (mechGimbalControls != null)
            {
                float val = Mathf.Clamp((mechGimbalController.RotationSpeed - minMaxLookSpeed.x) / (minMaxLookSpeed.y - minMaxLookSpeed.x), 0, 1);
                lookSensitivitySlider.value = val;

            }
        }
    }
}
