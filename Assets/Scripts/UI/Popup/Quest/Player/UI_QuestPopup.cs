using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class UI_QuestPopup : UI_Popup, ISavable
{
    public static string SaveKey => "SaveQuestUI";

    enum GameObjects
    {
        QuestInfo,
    }

    enum RectTransforms
    {
        QuestTitleSubitems,
        QuestRewardSlots,
    }

    enum Texts
    {
        QuestTitleText,
        QuestDescriptionText,
        QuestTargetText,
        QuestRewardText,
        NOQuestText,
    }

    enum Buttons
    {
        CloseButton,
        CompleteButton,
        CancelButton,
    }

    private Quest _questRef;
    private readonly Dictionary<Quest, UI_QuestSubitem> _titleSubitems = new();
    private readonly StringBuilder _sb = new(50);

    protected override void Init()
    {
        base.Init();

        BindObject(typeof(GameObjects));
        BindRT(typeof(RectTransforms));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_QuestPopup>);
        GetButton((int)Buttons.CompleteButton).onClick.AddListener(() => Managers.Quest.Complete(_questRef));
        GetButton((int)Buttons.CancelButton).onClick.AddListener(() => Managers.Quest.Unregister(_questRef));

        Managers.Quest.QuestRegistered += OnQuestRegisterd;
        Managers.Quest.QuestUnregistered += OnQuestCompletedOrCanceled;
        Managers.Quest.QuestCompletabled += OnQuestCompletabled;
        Managers.Quest.QuestCompletableCanceled += OnQuestCompletableCanceld;
        Managers.Quest.QuestCompleted += OnQuestCompletedOrCanceled;

        Clear();
    }

    private void Start()
    {
        Managers.UI.Register<UI_QuestPopup>(this);

        foreach (var quest in Managers.Quest.ActiveQuests)
        {
            OnQuestRegisterd(quest);
            if (quest.State == QuestState.Completable)
            {
                if (_titleSubitems.TryGetValue(quest, out var subitem))
                {
                    subitem.SetActiveCompleteText(true);
                    SetActiveCompleteButton(quest, _questRef == quest);
                }
            }
        }

        Load();
    }

    public void SetQuest(Quest quest)
    {
        if (quest == null)
        {
            return;
        }

        if (_questRef == quest)
        {
            return;
        }

        Clear();

        _questRef = quest;
        quest.TargetCountChanged += RefreshTargetText;
        GetObject((int)GameObjects.QuestInfo).SetActive(true);
        GetText((int)Texts.QuestTitleText).text = quest.Data.QuestName;
        GetText((int)Texts.QuestDescriptionText).text = quest.Data.Description;
        GetText((int)Texts.NOQuestText).gameObject.SetActive(false);
        GetButton((int)Buttons.CancelButton).gameObject.SetActive(true);
        SetActiveCompleteButton(quest, quest.State == QuestState.Completable);
        RefreshTargetText();
        SetReward(quest.Data);
    }

    public void SetActiveQuestTracker(Quest quest, bool active)
    {
        if (_titleSubitems.TryGetValue(quest, out var subitem))
        {
            subitem.SetActiveQuestTracker(active);
        }
    }

    public JToken GetSaveData()
    {
        var saveData = new JArray();

        foreach (var kvp in _titleSubitems)
        {
            if (!kvp.Value.IsShowedTracker())
            {
                continue;
            }

            saveData.Add(kvp.Key.Data.QuestId);
        }

        return saveData;
    }

    private void OnQuestRegisterd(Quest quest)
    {
        var go = Managers.Resource.Instantiate("UI_QuestSubitem.prefab", GetRT((int)RectTransforms.QuestTitleSubitems), true);
        var subitem = go.GetComponent<UI_QuestSubitem>();
        subitem.SetQuest(quest);
        _titleSubitems.Add(quest, subitem);
    }

    private void OnQuestCompletabled(Quest quest)
    {
        if (_titleSubitems.TryGetValue(quest, out var subitem))
        {
            subitem.SetActiveCompleteText(true);
            SetActiveCompleteButton(quest, _questRef == quest);
        }
    }

    private void OnQuestCompletableCanceld(Quest quest)
    {
        if (_titleSubitems.TryGetValue(quest, out var subitem))
        {
            subitem.SetActiveCompleteText(false);
            SetActiveCompleteButton(quest, !(_questRef == quest));
        }
    }

    private void OnQuestCompletedOrCanceled(Quest quest)
    {
        if (_titleSubitems.TryGetValue(quest, out var subitem))
        {
            subitem.SetActiveCompleteText(false);
            subitem.SetActiveQuestTracker(false);
            Managers.Resource.Destroy(subitem.gameObject);
            _titleSubitems.Remove(quest);
            if (_questRef == quest)
            {
                Clear();
            }
        }
    }

    private void RefreshTargetText()
    {
        _sb.Clear();
        _sb.AppendLine("[목적]");

        foreach (var kvp in _questRef.Targets)
        {
            int completeCount = kvp.Key.CompleteCount;
            int currentCount = Mathf.Clamp(kvp.Value, 0, completeCount);
            _sb.AppendLine($"- {kvp.Key.Description} ({currentCount}/{completeCount})");
        }

        GetText((int)Texts.QuestTargetText).text = _sb.ToString();
    }

    private void SetReward(QuestData questData)
    {
        _sb.Clear();
        _sb.AppendLine("[보상]");

        if (questData.RewardGold > 0)
        {
            _sb.AppendLine($"{questData.RewardGold} Gold");
        }

        if (questData.RewardXP > 0)
        {
            _sb.AppendLine($"{questData.RewardXP} XP");
        }

        GetText((int)Texts.QuestRewardText).text = _sb.ToString();

        foreach (var kvp in questData.RewardItems)
        {
            var go = Managers.Resource.Instantiate("UI_QuestRewardSlot.prefab", GetRT((int)RectTransforms.QuestRewardSlots), true);
            go.GetComponent<UI_QuestRewardSlot>().SetReward(kvp.Key, kvp.Value);
        }
    }

    private void SetActiveCompleteButton(Quest quest, bool active)
    {
        if (active && quest.Data.CanRemoteComplete)
        {
            GetButton((int)Buttons.CompleteButton).gameObject.SetActive(true);
        }
        else
        {
            GetButton((int)Buttons.CompleteButton).gameObject.SetActive(false);
        }
    }

    private void Clear()
    {
        if (_questRef != null)
        {
            _questRef.TargetCountChanged -= RefreshTargetText;
            _questRef = null;
        }

        GetObject((int)GameObjects.QuestInfo).SetActive(false);
        GetText((int)Texts.NOQuestText).gameObject.SetActive(true);
        GetButton((int)Buttons.CompleteButton).gameObject.SetActive(false);
        GetButton((int)Buttons.CancelButton).gameObject.SetActive(false);

        foreach (Transform slot in GetRT((int)RectTransforms.QuestRewardSlots))
        {
            if (slot.gameObject == GetText((int)Texts.QuestRewardText).gameObject)
            {
                continue;
            }

            Managers.Resource.Destroy(slot.gameObject);
        }
    }

    private void Load()
    {
        if (!Managers.Data.Load<JArray>(SaveKey, out var saveData))
        {
            return;
        }

        foreach (var token in saveData)
        {
            var questId = token.ToString();

            foreach (var kvp in _titleSubitems)
            {
                if (kvp.Key.Data.QuestId.Equals(questId))
                {
                    kvp.Value.SetActiveQuestTracker(true);
                }
            }
        }
    }
}
