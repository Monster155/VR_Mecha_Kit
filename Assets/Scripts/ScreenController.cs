using Aiming;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    [SerializeField] private Transform _headTransform;
    [SerializeField] private LayerMask _screenLayer;
    [SerializeField] private AimingBase _aiming;
    [SerializeField] private Transform _targetImage;

    private void Update()
    {
        Ray ray = new Ray(_headTransform.position, _aiming.HitPoint - _headTransform.position);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _screenLayer))
        {
            _targetImage.gameObject.SetActive(true);

            _targetImage.position = hit.point;
        }
        else
        {
            _targetImage.gameObject.SetActive(false);
        }
    }
}