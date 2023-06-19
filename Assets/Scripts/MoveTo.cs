using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MoveTo : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Rigidbody _rigidbody;

    private bool _isSelected;

    void Update()
    {
        if (_isSelected)
        {
            _rigidbody.MovePosition(_target.position);
            // if (Vector3.Distance(_target.position, transform.position) > 0.4f)
            // {
            //     grabbedBy.ForceRelease(this);
            // }
        }
        else
        {
            _target.position = transform.position;
            _target.rotation = transform.rotation;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }

    public void IsTargetSelected(bool isSelected)
    {
        _isSelected = isSelected;
    }
}