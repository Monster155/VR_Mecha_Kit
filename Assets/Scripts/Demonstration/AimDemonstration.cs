using Aiming;
using CabinRotation;
using TMPro;
using UnityEngine;

namespace Demonstration
{
    public class AimDemonstration : MonoBehaviour
    {
        [SerializeField] private AimingBase _aimingBase;
        [SerializeField] private TMP_Text _valueText;

        private void Update()
        {
            _valueText.text = _aimingBase.IsHit + " : " + _aimingBase.HitPoint;
        }
    }
}