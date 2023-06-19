using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UniversalVehicleCombat;

namespace VSX.UniversalVehicleCombat
{
    public class CameraZoomControls : GeneralInput
    {
        [SerializeField]
        protected VehicleCamera m_Camera;

        [SerializeField]
        protected float minFOV;

        [SerializeField]
        protected float zoomSpeed;

        protected float currentFOV;


        protected override void Start()
        {
            base.Start();

            // Store the starting FOV of the camera
            if (m_Camera != null) currentFOV = m_Camera.DefaultFieldOfView;
        }

        protected override void InputUpdate()
        {
            if (m_Camera == null) return;

            // Update the FOV
            currentFOV = Mathf.Clamp(currentFOV - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime, minFOV, m_Camera.DefaultFieldOfView);

            // Set the FOV
            m_Camera.SetFieldOfView(currentFOV);
        }
    }
}

