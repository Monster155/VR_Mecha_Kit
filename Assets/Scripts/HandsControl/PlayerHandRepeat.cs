using System;
using UnityEngine;

namespace HandsControl
{
    public class PlayerHandRepeat : MonoBehaviour
    {
        [SerializeField] private Transform _playerLeftHand;
        [SerializeField] private Transform _playerRightHand;
        [SerializeField] private Transform _playerHead;
        [SerializeField] private Transform _playerRoot;
        [SerializeField] private Transform _robotLeftHand;
        [SerializeField] private Transform _robotRightHand;
        [SerializeField] private Transform _robotHead;
        [SerializeField] private Transform _robotRoot;

        private float _scale;

        private void Start()
        {
            float playerHeight = _playerHead.position.z - _playerRoot.position.z;
            float robotHeight = _robotHead.position.z - _robotRoot.position.z;

            _scale = robotHeight / playerHeight;
        }

        private void Update()
        {
            // TODO make calibration
            // _playerLeftHand
            // _playerRightHand
            // _playerRoot
        }
    }
}