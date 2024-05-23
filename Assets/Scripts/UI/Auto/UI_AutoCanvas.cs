using UnityEngine;

public class UI_AutoCanvas : UI_Base
{
    protected override void Init()
    {
        Managers.UI.Register<UI_AutoCanvas>(this);
    }

    public void AddAutoUI(UI_Auto auto)
    {
        auto.transform.SetParent(transform);
        auto.gameObject.SetActive(false);
    }
}
