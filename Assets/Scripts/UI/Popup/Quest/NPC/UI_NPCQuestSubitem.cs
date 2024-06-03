using UnityEngine;

public class UI_NPCQuestSubitem : UI_Base
{
    enum Texts
    {
        TitleText,
        CompleteText,
    }

    enum Buttons
    {
        TitleButton,
    }

    public QuestData QuestDataRef { get; private set; }

    protected override void Init()
    {
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetText((int)Texts.CompleteText).gameObject.SetActive(false);
        GetButton((int)Buttons.TitleButton).onClick.AddListener(() => Managers.UI.Get<UI_NPCQuestPopup>().SetQuestDescription(QuestDataRef));
    }

    public void SetQuestData(QuestData questData)
    {
        QuestDataRef = questData;
        GetText((int)Texts.TitleText).text = $"[{questData.LimitLevel}] {questData.QuestName}";
        var quest = Managers.Quest.GetActiveQuest(questData);
        if (quest != null && quest.State == QuestState.Completable)
        {
            GetText((int)Texts.CompleteText).gameObject.SetActive(true);
        }
        else
        {
            GetText((int)Texts.CompleteText).gameObject.SetActive(false);
        }
    }

    public void SetActiveCompleteText(bool active)
    {
        GetText((int)Texts.CompleteText).gameObject.SetActive(active);
    }
}
