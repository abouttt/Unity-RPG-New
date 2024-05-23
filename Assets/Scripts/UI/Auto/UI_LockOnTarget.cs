using UnityEngine;

public class UI_LockOnTarget : UI_Auto
{
    public Transform Target
    {
        get => _followWorldObject.Target;
        set => _followWorldObject.Target = value;
    }

    private UI_FollowWorldObject _followWorldObject;

    protected override void Init()
    {
        _followWorldObject = GetComponent<UI_FollowWorldObject>();
    }
}
