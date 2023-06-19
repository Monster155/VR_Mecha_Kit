using UnityEngine;

namespace Movement.Gearbox
{
    public class SpeedControllerScreen : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer;
        [Space]
        [SerializeField] private Material _parking;
        [SerializeField] private Material _reverse;
        [SerializeField] private Material _neutral;
        [SerializeField] private Material _drive;

        public void ChangeState(SpeedState state)
        {
            _renderer.material = state switch
            {
                SpeedState.Parking => _parking,
                SpeedState.Reverse => _reverse,
                SpeedState.Neutral => _neutral,
                SpeedState.Drive => _drive,
                _ => _renderer.material
            };
        }
    }
}