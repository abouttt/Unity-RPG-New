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
            if (IsLockOn && value == null)
            {
                LockedTarget.GetComponent<Lockable>().IsLockOn = false;
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

    private Transform _lockedTarget;

    private readonly float _threshold = 0.01f;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private GameObject _mainCamera;
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
        var saveData = new JArray();
        var vector3SaveData = new Vector3SaveData(_cinemachineCameraTarget.rotation.eulerAngles);
        saveData.Add(JObject.FromObject(vector3SaveData));
        return saveData;
    }

    private void CameraRotation()
    {
        if (IsLockOn)
        {
            _cinemachineCameraTarget.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0f);

            var direction = (_lockedTarget.position + transform.position) * 0.5f;
            var targetRotation = Quaternion.LookRotation(direction - _cinemachineCameraTarget.position);
            var rotation = Quaternion.Slerp(_cinemachineCameraTarget.rotation, targetRotation, _lockOnRotationSpeed * Time.deltaTime);
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
        _cinemachineCameraTarget.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0f);
    }

    private void FindLockableTarget()
    {
        float shortestAngle = Mathf.Infinity;
        Transform finalTarget = null;

        var targets = Physics.OverlapSphere(_mainCamera.transform.position, _viewRadius, _targetMask);
        foreach (var target in targets)
        {
            var dirToTarget = (target.transform.position - _mainCamera.transform.position).normalized;
            float currentAngle = Vector3.Angle(_mainCamera.transform.forward, dirToTarget);
            if (currentAngle < _viewAngle * 0.5f)
            {
                if (currentAngle < shortestAngle)
                {
                    float distToTarget = Vector3.Distance(_mainCamera.transform.position, target.transform.position);
                    if (!Physics.Raycast(_mainCamera.transform.position, dirToTarget, distToTarget, _obstacleMask))
                    {
                        finalTarget = target.transform;
                        shortestAngle = currentAngle;
                    }
                }
            }
        }

        LockedTarget = finalTarget;
    }

    private void TrackingLockedTarget()
    {
        if (!LockedTarget.gameObject.activeInHierarchy)
        {
            LockedTarget = null;
            return;
        }

        float targetDistance = Vector3.Distance(_mainCamera.transform.position, LockedTarget.position);
        if (targetDistance > _viewRadius)
        {
            LockedTarget = null;
            return;
        }

        var targetDirection = (LockedTarget.position - _mainCamera.transform.position).normalized;
        if (Physics.Raycast(_mainCamera.transform.position, targetDirection, targetDistance, _obstacleMask))
        {
            LockedTarget = null;
            return;
        }

        float pitch = Util.ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);
        if (_bottomClamp > pitch || pitch > _topClamp)
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
