using UnityEngine;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;

public class UI_ItemSplitPopup : UI_Popup
{
    enum GameObjects
    {
        ItemPrice,
    }

    enum Texts
    {
        GuideText,
        PriceText,
    }

    enum Buttons
    {
        UpButton,
        DownButton,
        YesButton,
        NoButton,
    }

    enum InputFields
    {
        InputField,
    }

    public int Count { get; private set; }

    private int _price;
    private int _minCount;
    private int _maxCount;
    private bool _isShowedPrice;

    private DOTweenAnimation _dotween;

    protected override void Init()
    {
        base.Init();

        _dotween = PopupRT.GetComponent<DOTweenAnimation>();

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        Bind<TMP_InputField>(typeof(InputFields));

        Get<TMP_InputField>((int)InputFields.InputField).onValueChanged.AddListener(value => OnValueChanged(value));
        Get<TMP_InputField>((int)InputFields.InputField).onEndEdit.AddListener(value => OnEndEdit(value));
        Get<TMP_InputField>((int)InputFields.InputField).onSubmit.AddListener(value => GetButton((int)Buttons.YesButton).onClick.Invoke());

        GetButton((int)Buttons.UpButton).onClick.AddListener(() => OnClickUpOrDownButton(1));
        GetButton((int)Buttons.DownButton).onClick.AddListener(() => OnClickUpOrDownButton(-1));
        GetButton((int)Buttons.NoButton).onClick.AddListener(Managers.UI.Close<UI_ItemSplitPopup>);
    }

    private void Start()
    {
        Managers.UI.Register<UI_ItemSplitPopup>(this);

        Showed += () =>
        {
            PopupRT.localScale = new Vector3(0f, 1f, 1f);
            _dotween.DORestart();
        };
    }

    public void SetEvent(UnityAction callback, string text, int minCount, int maxCount, int price = 0, bool showPrice = false)
    {
        GetButton((int)Buttons.YesButton).onClick.RemoveAllListeners();
        GetButton((int)Buttons.YesButton).onClick.AddListener(callback);
        GetButton((int)Buttons.YesButton).onClick.AddListener(Managers.UI.Close<UI_ItemSplitPopup>);

        Count = maxCount;
        _maxCount = maxCount;
        _minCount = minCount;
        _price = price;
        _isShowedPrice = showPrice;

        GetText((int)Texts.GuideText).text = text;
        Get<TMP_InputField>((int)InputFields.InputField).text = Count.ToString();
        Get<TMP_InputField>((int)InputFields.InputField).ActivateInputField();
        GetObject((int)GameObjects.ItemPrice).SetActive(_isShowedPrice);
        RefreshPriceText();
    }

    private void OnValueChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Count = 0;
        }
        else
        {
            Count = Mathf.Clamp(int.Parse(value), _minCount, _maxCount);
            Get<TMP_InputField>((int)InputFields.InputField).text = Count.ToString();
        }

        RefreshPriceText();
    }

    private void OnEndEdit(string value)
    {
        Count = Mathf.Clamp(string.IsNullOrEmpty(value) ? _maxCount : int.Parse(value), _minCount, _maxCount);
        var inputField = Get<TMP_InputField>((int)InputFields.InputField);
        inputField.text = Count.ToString();
        inputField.caretPosition = inputField.text.Length;
    }

    private void OnClickUpOrDownButton(int count)
    {
        Count = Mathf.Clamp(Count + count, _minCount, _maxCount);
        Get<TMP_InputField>((int)InputFields.InputField).text = Count.ToString();
    }

    private void RefreshPriceText()
    {
        if (!_isShowedPrice)
        {
            return;
        }

        int totalPrice = _price * Count;
        GetText((int)Texts.PriceText).text = totalPrice.ToString();
        GetText((int)Texts.PriceText).color = _price * Count <= Player.Status.Gold ? Color.white : Color.red;
    }
}
