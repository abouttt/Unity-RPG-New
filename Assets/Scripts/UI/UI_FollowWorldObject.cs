using UnityEngine;

public class UI_FollowWorldObject : MonoBehaviour
{
    public Transform Target
    {
        get => _target;
        set
        {
            _target = value;
            gameObject.SetActive(_target != null);
        }
    }

    private Transform _target;
    private RectTransform _rt;
    private Camera _mainCamera;

    [field: SerializeField]
    public Vector3 Offset { get; set; }

    private void Awake()
    {
        _mainCamera = Camera.main;
        _rt = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (_target == null)
        {
            gameObject.SetActive(false);
            return;
        }

        _rt.position = _mainCamera.WorldToScreenPoint(_target.position + Offset);
    }

    public void SetTargetAndOffset(Transform target, Vector3 offset)
    {
        Target = target;
        Offset = offset;
    }
}
