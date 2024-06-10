using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [field: SerializeField]
    public Transform Top { get; private set; }

    [field: SerializeField]
    public Transform Middle { get; private set; }

    [field: SerializeField]
    public Transform Bottom { get; private set; }

    public bool Enabled
    {
        get => _collider.enabled;
        set
        {
            _collider.enabled = value;
            if (!value)
            {
                _hittedColliders.Clear();
            }
        }
    }

    private Collider _collider;
    private readonly HashSet<Collider> _hittedColliders = new();

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Weapon");
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
        _collider.enabled = false;
    }
}
