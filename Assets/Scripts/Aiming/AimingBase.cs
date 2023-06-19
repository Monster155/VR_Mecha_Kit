using UnityEngine;

namespace Aiming
{
    public class AimingBase : MonoBehaviour
    {
        public Vector3 HitPoint { get; protected set; }
        public bool IsHit { get; protected set; }
        
        [SerializeField] private Transform _shootingPointTransform;
        [SerializeField] private float _maxDistance = 1000f;

        private void Update()
        {
            Ray ray = new Ray(_shootingPointTransform.position, _shootingPointTransform.forward);
            bool isHit = Physics.Raycast(ray, out RaycastHit hit, _maxDistance);

            HitPoint = isHit ? hit.point : _shootingPointTransform.position + _shootingPointTransform.forward * _maxDistance;
            IsHit = isHit;
        }
    }
}