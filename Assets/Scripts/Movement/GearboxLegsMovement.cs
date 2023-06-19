using System;
using System.Collections.Generic;
using Movement.Gearbox;
using UnityEngine;

namespace Movement
{
    public class GearboxLegsMovement : LegsMovementBase
    {
        [SerializeField] private Transform _joystickTransform;
        [Space]
        [SerializeField] private Transform _parking;
        [SerializeField] private Transform _reverse;
        [SerializeField] private Transform _neutral;
        [SerializeField] private Transform _drive;
        [Space]
        [SerializeField] private SpeedControllerScreen _screen;

        private SpeedState _state = SpeedState.Parking;
        private bool _isInHand = false;

        private Dictionary<SpeedState, Transform> _speedRange;
        private void Start()
        {
            _speedRange = new Dictionary<SpeedState, Transform>
            {
                { SpeedState.Parking, _parking },
                { SpeedState.Reverse, _reverse },
                { SpeedState.Neutral, _neutral },
                { SpeedState.Drive, _drive },
            };
            _joystickTransform.position = _speedRange[_state].position;
        }

        private void Update()
        {
            GetClosestState();

            _screen.ChangeState(_state);

            if(!_isInHand)
                _joystickTransform.position = _state switch
                {
                    SpeedState.Parking => _parking.position,
                    SpeedState.Reverse => _reverse.position,
                    SpeedState.Neutral => _neutral.position,
                    SpeedState.Drive => _drive.position,
                    _ => _joystickTransform.position
                };

            Power = _state switch
            {
                SpeedState.Reverse => new Vector3(0, 0, -1),
                SpeedState.Drive => new Vector3(0, 0, 1),
                _ => Vector3.zero
            };
        }

        private void GetClosestState()
        {
            SpeedState closestState = _state;
            float closestRange = Math.Abs(Vector3.Distance(_speedRange[closestState].position, _joystickTransform.position));

            foreach (SpeedState state in Enum.GetValues(typeof(SpeedState)))
            {
                float newRange = Math.Abs(Vector3.Distance(_speedRange[state].position, _joystickTransform.position));
                if (closestRange > newRange)
                {
                    closestRange = newRange;
                    closestState = state;
                }
            }

            _state = closestState;
        }
    }
}