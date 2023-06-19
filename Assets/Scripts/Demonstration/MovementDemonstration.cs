using System;
using Movement;
using TMPro;
using UnityEngine;

namespace Demonstration
{
    public class MovementDemonstration : MonoBehaviour
    {
        [SerializeField] private LegsMovementBase _movementBase;
        [SerializeField] private TMP_Text _valueText;

        private void Update()
        {
            _valueText.text = _movementBase.Power.ToString();
        }
    }
}