using UnityEngine;
using UnityEngine.Events;

public class UI_NPCMenuSubitem : UI_Base
{
    enum Texts
    {
        MenuNameText,
    }

    enum Buttons
    {
        Button,
    }

    protected override void Init()
    {
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
    }

    public void SetEvent(string menuName, UnityAction callback)
    {
        GetText((int)Texts.MenuNameText).text = menuName;
        GetButton((int)Buttons.Button).onClick.AddListener(callback);
    }

    public void Clear()
    {
        GetButton((int)Buttons.Button).onClick.RemoveAllListeners();
    }
}
