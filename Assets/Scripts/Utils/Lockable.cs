using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Lockable : MonoBehaviour
{
    public Action<bool> LockChanged;

    public bool IsLockOn
    {
        get => _isLockOn;
        set
        {
            if (_isLockOn != value)
            {
                _isLockOn = value;
                LockChanged?.Invoke(_isLockOn);
            }
        }
    }

    private bool _isLockOn = false;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Lockable");
    }
}
