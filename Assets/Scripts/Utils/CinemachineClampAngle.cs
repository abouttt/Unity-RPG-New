using UnityEngine;
using Cinemachine;

public class CinemachineClampAngle : CinemachineExtension
{
    [SerializeField]
    private float _topClamp;

    [SerializeField]
    private float _bottomClamp;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Aim)
        {
            var eulerAngles = state.RawOrientation.eulerAngles;
            eulerAngles.x = Util.ClampAngle(eulerAngles.x, _bottomClamp, _topClamp);
            eulerAngles.y = Util.ClampAngle(eulerAngles.y, float.MinValue, float.MaxValue);
            state.RawOrientation = Quaternion.Euler(eulerAngles);
        }
    }
}
