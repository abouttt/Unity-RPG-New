using UnityEngine;

public abstract class UI_Auto : UI_Base
{
    protected virtual void Start()
    {
        Managers.UI.Get<UI_AutoCanvas>().AddAutoUI(this);
    }
}
