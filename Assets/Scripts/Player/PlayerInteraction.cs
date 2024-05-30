using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float InputTime { get; private set; }
    public bool IsShowedKeyGuide => _keyGuide.gameObject.activeSelf;

    private Interactive _target;
    private UI_InteractionKeyGuide _keyGuide;
    private GameObject _mainCamera;
    private bool _isRangeOutTarget;
    private bool _canInteraction;

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
    }

    private void Start()
    {
        var go = Managers.Resource.Instantiate("UI_InteractionKeyGuide.prefab");
        _keyGuide = go.GetComponent<UI_InteractionKeyGuide>();
    }

    private void Update()
    {
        if (_target == null)
        {
            return;
        }

        if (!_target.gameObject.activeSelf)
        {
            SetTarget(null);
            return;
        }

        if (_target.IsInteracted)
        {
            _canInteraction = false;
            return;
        }
        else if (_isRangeOutTarget)
        {
            SetTarget(null);
            return;
        }

        if (Managers.Input.Interaction)
        {
            if (_canInteraction && _target.CanInteraction)
            {
                InputTime += Time.deltaTime;
                if (InputTime >= _target.InteractionInputTime)
                {
                    InputTime = 0f;
                    _keyGuide.gameObject.SetActive(false);
                    _target.IsInteracted = true;
                    _target.Interaction();
                }
            }
        }
        else
        {
            InputTime = 0f;
            _canInteraction = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_target == null)
        {
            SetTarget(other.GetComponent<Interactive>());
        }
        else
        {
            if (_target.IsInteracted)
            {
                return;
            }
            else if (!_keyGuide.gameObject.activeSelf)
            {
                _keyGuide.gameObject.SetActive(true);
            }

            if (InputTime > 0f)
            {
                return;
            }

            if (_target.gameObject != other.gameObject)
            {
                var cameraToTarget = _target.transform.position - _mainCamera.transform.position;
                var cameraToOther = other.transform.position - _mainCamera.transform.position;
                float targetAngle = Vector3.Angle(cameraToTarget, _mainCamera.transform.forward);
                float otherAngle = Vector3.Angle(cameraToOther, _mainCamera.transform.forward);
                if (otherAngle < targetAngle)
                {
                    SetTarget(other.GetComponent<Interactive>());
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_target == null)
        {
            return;
        }

        if (_target.gameObject != other.gameObject)
        {
            return;
        }

        if (_target.IsInteracted)
        {
            _isRangeOutTarget = true;
        }
        else
        {
            SetTarget(null);
        }
    }

    private void SetTarget(Interactive target)
    {
        if (_target != null)
        {
            _target.IsDetected = false;
        }

        InputTime = 0f;
        _target = target;
        _keyGuide.SetTarget(target);
        _isRangeOutTarget = false;
        _canInteraction = false;

        if (_target != null)
        {
            _target.IsDetected = true;
        }
    }
}
