using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager
{
    public event Action<Quest> QuestRegistered;
    public event Action<Quest> QuestUnregistered;
    public event Action<Quest> QuestCompletabled;
    public event Action<Quest> QuestCompletableCanceled;
    public event Action<Quest> QuestCompleted;

    public IReadOnlyList<Quest> ActiveQuests => _activeQuests;
    public IReadOnlyList<Quest> CompletedQuests => _completedQuests;

    private readonly List<Quest> _activeQuests = new();
    private readonly List<Quest> _completedQuests = new();

    public Quest Register(QuestData questData)
    {
        var newQuest = new Quest(questData);
        _activeQuests.Add(newQuest);
        NPC.TryRemoveQuestToNPC(questData.OwnerId, questData);
        QuestRegistered?.Invoke(newQuest);

        if (newQuest.State == QuestState.Completable)
        {
            NPC.TryAddQuestToNPC(questData.CompleteOwnerId, questData);
            QuestCompletabled?.Invoke(newQuest);
        }

        return newQuest;
    }

    public void Unregister(Quest quest)
    {
        if (quest == null)
        {
            return;
        }

        if (_activeQuests.Remove(quest))
        {
            var prevState = quest.State;
            quest.Cancel();

            if (prevState == QuestState.Completable)
            {
                NPC.TryRemoveQuestToNPC(quest.Data.CompleteOwnerId, quest.Data);
                QuestCompletableCanceled?.Invoke(quest);
            }

            NPC.TryAddQuestToNPC(quest.Data.OwnerId, quest.Data);
            QuestUnregistered?.Invoke(quest);
            ReceiveReport(Category.Quest, quest.Data.QuestId, -1);
        }
    }

    public void ReceiveReport(Category category, string id, int count)
    {
        if (count == 0)
        {
            return;
        }

        foreach (var quest in _activeQuests)
        {
            var prevState = quest.State;

            if (quest.ReceiveReport(category, id, count))
            {
                if (quest.State == QuestState.Completable)
                {
                    if (prevState != QuestState.Completable)
                    {
                        NPC.TryAddQuestToNPC(quest.Data.CompleteOwnerId, quest.Data);
                        QuestCompletabled?.Invoke(quest);
                    }
                }
                else if (prevState == QuestState.Completable)
                {
                    NPC.TryRemoveQuestToNPC(quest.Data.CompleteOwnerId, quest.Data);
                    QuestCompletableCanceled?.Invoke(quest);
                }
            }
        }
    }

    public void Complete(Quest quest)
    {
        if (quest == null)
        {
            return;
        }

        if (quest.Complete())
        {
            _activeQuests.Remove(quest);
            _completedQuests.Add(quest);

            NPC.TryRemoveQuestToNPC(quest.Data.CompleteOwnerId, quest.Data);
            QuestCompleted?.Invoke(quest);
            ReceiveReport(Category.Quest, quest.Data.QuestId, 1);
        }
    }

    public Quest GetActiveQuest(QuestData questData)
    {
        return _activeQuests.Find(quest => quest.Data.Equals(questData));
    }

    public Quest GetCompleteQuest(QuestData questData)
    {
        return _completedQuests.Find(quest => quest.Data.Equals(questData));
    }

    public void Clear()
    {
        foreach (var quest in _activeQuests)
        {
            quest.Cancel();
        }

        _activeQuests.Clear();
        _completedQuests.Clear();

        QuestRegistered = null;
        QuestUnregistered = null;
        QuestCompletabled = null;
        QuestCompletableCanceled = null;
        QuestCompleted = null;
    }
}
