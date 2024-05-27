using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class UI_ConfirmationPopup : UI_Popup
{
    enum Texts
    {
        GuideText,
        YesText,
        NoText,
    }

    enum Buttons
    {
        YesButton,
        NoButton,
    }

    private DOTweenAnimation _dotween;

    protected override void Init()
    {
        base.Init();

        _dotween = PopupRT.GetComponent<DOTweenAnimation>();

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.NoButton).onClick.AddListener(Managers.UI.Close<UI_ConfirmationPopup>);
    }

    private void Start()
    {
        Managers.UI.Register<UI_ConfirmationPopup>(this);

        Showed += () =>
        {
            PopupRT.localScale = new Vector3(0f, 1f, 1f);
            _dotween.DORestart();
        };
    }

    // 예 버튼에 이벤트 설정.
    public void SetEvent(UnityAction callback, string text, string yesText = "예", string noText = "아니오", bool yes = true, bool no = true)
    {
        GetButton((int)Buttons.YesButton).onClick.RemoveAllListeners();
        GetButton((int)Buttons.YesButton).onClick.AddListener(callback);
        GetButton((int)Buttons.YesButton).onClick.AddListener(Managers.UI.Close<UI_ConfirmationPopup>);
        GetButton((int)Buttons.YesButton).gameObject.SetActive(yes);
        GetButton((int)Buttons.NoButton).gameObject.SetActive(no);
        GetText((int)Texts.YesText).text = yesText;
        GetText((int)Texts.NoText).text = noText;
        GetText((int)Texts.GuideText).text = text;
    }
}
