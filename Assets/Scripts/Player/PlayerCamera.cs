using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json.Linq;

public class PlayerCamera : MonoBehaviour, ISavable
{
    public static string SaveKey => "SaveCamera";

    public Transform LockedTarget
    {
        get => _lockedTarget;
        set
        {
            if (value == null)
            {
                if (IsLockOn)
                {
                    LockedTarget.GetComponent<Lockable>().IsLockOn = false;
                }
                else
                {
                    return;
                }
            }

            IsLockOn = value != null;
            _lockedTarget = value;
            _lockOnTargetImage.Target = value;

            if (IsLockOn)
            {
                value.GetComponent<Lockable>().IsLockOn = true;
            }
        }
    }

    public bool IsLockOn { get; private set; }

    [Header("[Rotate]")]
    [SerializeField]
    private Transform _cinemachineCameraTarget;

    [SerializeField]
    private float _sensitivity;

    [SerializeField]
    private float _topClamp;

    [SerializeField]
    private float _bottomClamp;

    [Header("[Lock on target]")]
    [SerializeField]
    private float _lockOnRotationSpeed;

    [SerializeField]
    private float _viewRadius;

    [Range(0, 360)]
    [SerializeField]
    private float _viewAngle;

    [SerializeField]
    private LayerMask _targetMask;

    [SerializeField]
    private LayerMask _obstacleMask;

    private readonly float _threshold = 0.01f;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private Quaternion _currentRotation;
    private Quaternion _targetRotation;

    private GameObject _mainCamera;
    private Transform _lockedTarget;
    private UI_LockOnTarget _lockOnTargetImage;

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;

        if (!Managers.Game.IsDefaultSpawn && !Managers.Game.IsPortalSpawn)
        {
            Load();
        }
    }

    private void Start()
    {
        var go = Managers.Resource.Instantiate("UI_LockOnTarget.prefab");
        _lockOnTargetImage = go.GetComponent<UI_LockOnTarget>();

        _cinemachineTargetPitch = _cinemachineCameraTarget.rotation.eulerAngles.x;
        _cinemachineTargetYaw = _cinemachineCameraTarget.rotation.eulerAngles.y;

        Managers.Input.GetAction("LockOn").performed += FindTargetOrReset;
    }

    private void LateUpdate()
    {
        CameraRotation();

        if (IsLockOn)
        {
            TrackingLockedTarget();
        }
    }

    public JToken GetSaveData()
    {
        var vector3SaveData = new Vector3SaveData(_cinemachineCameraTarget.rotation.eulerAngles);
        var saveData = new JArray(vector3SaveData);
        return saveData;
    }

    private void CameraRotation()
    {
        if (IsLockOn)
        {
            var direction = (_lockedTarget.position + transform.position) * 0.5f;
            _targetRotation = Quaternion.LookRotation(direction - _cinemachineCameraTarget.position);
            var rotation = Quaternion.Slerp(_currentRotation, _targetRotation, _lockOnRotationSpeed * Time.deltaTime);
            var euler = rotation.eulerAngles;
            _cinemachineTargetPitch = euler.x;
            _cinemachineTargetYaw = euler.y;
        }
        else
        {
            var look = Managers.Input.Look;
            if (look.sqrMagnitude >= _threshold)
            {
                _cinemachineTargetYaw += look.x * _sensitivity;
                _cinemachineTargetPitch += look.y * _sensitivity;
            }
        }

        _cinemachineTargetPitch = Util.ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);
        _cinemachineTargetYaw = Util.ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _currentRotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0f);
        _cinemachineCameraTarget.rotation = _currentRotation;
    }

    private void FindLockableTarget()
    {
        float shortestAngle = Mathf.Infinity;
        Transform finalTarget = null;

        var targets = Physics.OverlapSphere(_mainCamera.transform.position, _viewRadius, _targetMask);
        foreach (var target in targets)
        {
            var directionToTarget = (target.transform.position - _mainCamera.transform.position).normalized;
            float currentAngle = Vector3.Angle(_mainCamera.transform.forward, directionToTarget);
            if (_viewAngle >= currentAngle && currentAngle < shortestAngle)
            {
                if (Physics.Linecast(_mainCamera.transform.position, target.transform.position, _obstacleMask))
                {
                    continue;
                }

                finalTarget = target.transform;
                shortestAngle = currentAngle;
            }
        }

        LockedTarget = finalTarget;
    }

    private void TrackingLockedTarget()
    {
        bool canTracking = true;

        if (!_lockedTarget.gameObject.activeInHierarchy)
        {
            canTracking = false;
        }
        else if (Vector3.Distance(_mainCamera.transform.position, _lockedTarget.position) > _viewRadius)
        {
            canTracking = false;
        }
        else if (Physics.Linecast(_mainCamera.transform.position, _lockedTarget.position, _obstacleMask))
        {
            canTracking = false;
        }
        else
        {
            float pitch = Util.ClampAngle(_targetRotation.eulerAngles.x, _bottomClamp, _topClamp);
            if (_bottomClamp > pitch || pitch > _topClamp)
            {
                canTracking = false;
            }
        }

        if (!canTracking)
        {
            LockedTarget = null;
        }
    }

    private void FindTargetOrReset(InputAction.CallbackContext context)
    {
        if (IsLockOn)
        {
            LockedTarget = null;
        }
        else
        {
            FindLockableTarget();
        }
    }

    private void Load()
    {
        if (!Managers.Data.Load<JArray>(SaveKey, out var saveData))
        {
            return;
        }

        var vector3SaveData = saveData[0].ToObject<Vector3SaveData>();
        _cinemachineCameraTarget.rotation = Quaternion.Euler(vector3SaveData.ToVector3());
    }
}
