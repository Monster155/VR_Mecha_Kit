using CabinRotation;
using TMPro;
using UnityEngine;

namespace Demonstration
{
    public class RotationDemonstration : MonoBehaviour
    {
        [SerializeField] private CabinRotationBase _rotationBase;
        [SerializeField] private TMP_Text _valueText;

        private void Update()
        {
            _valueText.text = _rotationBase.Angle.ToString();
        }
    }
}