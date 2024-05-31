using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UI_NPCConversationPopup : UI_Popup, IPointerClickHandler
{
    enum Texts
    {
        NPCNameText,
        ScriptText,
    }

    enum Buttons
    {
        CloseButton,
    }

    [SerializeField]
    private float _typingSpeed;

    private NPCConversation _npcConversationRef;
    private int _index = 0;

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_NPCConversationPopup>);
    }

    private void Start()
    {
        Managers.UI.Register<UI_NPCConversationPopup>(this);

        Showed += () =>
        {
            Managers.UI.Get<UI_NPCMenuPopup>().PopupRT.gameObject.SetActive(false);
        };

        Closed += () =>
        {
            _npcConversationRef = null;
            GetText((int)Texts.ScriptText).DOKill();
            Managers.UI.Get<UI_NPCMenuPopup>().PopupRT.gameObject.SetActive(true);
        };
    }

    public void SetNPCConversation(NPCConversation npc)
    {
        _npcConversationRef = npc;
        _index = 0;
        GetText((int)Texts.NPCNameText).text = npc.Owner.NPCName;
        GetText((int)Texts.ScriptText).text = null;
        GetText((int)Texts.ScriptText).DOText(
            npc.ConversationScripts[_index], _npcConversationRef.ConversationScripts[_index].Length / _typingSpeed);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var script = GetText((int)Texts.ScriptText);
        if (script.text.Length == _npcConversationRef.ConversationScripts[_index].Length)
        {
            _index++;
            if (_index >= _npcConversationRef.ConversationScripts.Count)
            {
                Managers.UI.Close<UI_NPCConversationPopup>();
                return;
            }

            script.text = null;
            script.DOText(
                _npcConversationRef.ConversationScripts[_index], _npcConversationRef.ConversationScripts[_index].Length / _typingSpeed);
        }
        else
        {
            script.DOKill();
            script.text = _npcConversationRef.ConversationScripts[_index];
        }
    }
}
