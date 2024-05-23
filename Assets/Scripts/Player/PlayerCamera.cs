using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    public Transform LockedTarget
    {
        get => _stateDrivenCamera.LookAt;
        set
        {
            if (IsLockOn && value == null)
            {
                LockedTarget.GetComponent<Lockable>().IsLockOn = false;
            }

            IsLockOn = value != null;
            _stateDrivenCamera.LookAt = value;
            _cameraAnimator.SetBool(_animIDLockOn, IsLockOn);

            if (IsLockOn)
            {
                value.GetComponent<Lockable>().IsLockOn = true;
            }
        }
    }

    public bool IsLockOn { get; private set; }

    [Header("[Rotate]")]
    [SerializeField]
    private GameObject _cinemachineCameraTarget;

    [SerializeField]
    private float _sensitivity;

    [SerializeField]
    private float _topClamp;

    [SerializeField]
    private float _bottomClamp;

    [Header("[Lock on target]")]
    [SerializeField]
    private CinemachineStateDrivenCamera _stateDrivenCamera;

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

    private readonly int _animIDLockOn = Animator.StringToHash("LockOn");

    private GameObject _mainCamera;
    private Animator _cameraAnimator;
    private CinemachineComposer _targetComposer;

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
        _cameraAnimator = _stateDrivenCamera.GetComponent<Animator>();
        foreach (CinemachineVirtualCamera child in _stateDrivenCamera.ChildCameras)
        {
            _targetComposer = child.GetCinemachineComponent<CinemachineComposer>();
            if (_targetComposer != null)
            {
                break;
            }
        }
    }

    private void Start()
    {
        _cinemachineTargetPitch = _cinemachineCameraTarget.transform.rotation.eulerAngles.x;
        _cinemachineTargetYaw = _cinemachineCameraTarget.transform.rotation.eulerAngles.y;

        Managers.Input.GetAction("LockOn").performed += FindTargetOrReset;
    }

    private void LateUpdate()
    {
        CameraRotation();

        if (IsLockOn)
        {
            TrackingLockedTarget();
            CalcTrackedObjectOffset();
        }
    }

    private void CameraRotation()
    {
        if (IsLockOn)
        {
            _cinemachineCameraTarget.transform.rotation = _mainCamera.transform.rotation;
            var eulerAngles = _cinemachineCameraTarget.transform.eulerAngles;
            _cinemachineTargetPitch = eulerAngles.x;
            _cinemachineTargetYaw = eulerAngles.y;
        }
        else
        {
            var look = Managers.Input.Look;
            if (look.sqrMagnitude >= _threshold)
            {
                _cinemachineTargetYaw += look.x * _sensitivity;
                _cinemachineTargetPitch += look.y * _sensitivity;
            }

            _cinemachineTargetYaw = Util.ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = Util.ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);
            _cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0f);
        }
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

    private void CalcTrackedObjectOffset()
    {
        var lookAtPos = (LockedTarget.position + transform.position) * 0.5f;
        _targetComposer.m_TrackedObjectOffset.y = -lookAtPos.y;
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
}
