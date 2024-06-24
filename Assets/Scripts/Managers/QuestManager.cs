using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class QuestManager : ISavable
{
    public static string SaveKey => "SaveQuest";

    public event Action<Quest> QuestRegistered;
    public event Action<Quest> QuestUnregistered;
    public event Action<Quest> QuestCompletabled;
    public event Action<Quest> QuestCompletableCanceled;
    public event Action<Quest> QuestCompleted;

    public IReadOnlyList<Quest> ActiveQuests => _activeQuests;
    public IReadOnlyList<Quest> CompletedQuests => _completedQuests;

    private const string ACTIVE_QUEST_KEY = "SaveActiveQuest";
    private const string COMPLETE_QUEST_KEY = "SaveCompleteQuest";

    private readonly List<Quest> _activeQuests = new();
    private readonly List<Quest> _completedQuests = new();

    public Quest Register(QuestData questData)
    {
        var newQuest = new Quest(questData);
        _activeQuests.Add(newQuest);
        NPC.TryRemoveQuest(questData.OwnerId, questData);
        QuestRegistered?.Invoke(newQuest);

        if (newQuest.State == QuestState.Completable)
        {
            NPC.TryAddQuest(questData.CompleteOwnerId, questData);
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
                NPC.TryRemoveQuest(quest.Data.CompleteOwnerId, quest.Data);
                QuestCompletableCanceled?.Invoke(quest);
            }

            NPC.TryAddQuest(quest.Data.OwnerId, quest.Data);
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
                        NPC.TryAddQuest(quest.Data.CompleteOwnerId, quest.Data);
                        QuestCompletabled?.Invoke(quest);
                    }
                }
                else if (prevState == QuestState.Completable)
                {
                    NPC.TryRemoveQuest(quest.Data.CompleteOwnerId, quest.Data);
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

            NPC.TryRemoveQuest(quest.Data.CompleteOwnerId, quest.Data);
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

    public JToken GetSaveData()
    {
        var saveData = new JObject()
        {
            { ACTIVE_QUEST_KEY, CreateQuestsSaveData(_activeQuests) },
            { COMPLETE_QUEST_KEY, CreateQuestsSaveData(_completedQuests) },
        };

        return saveData;
    }

    public void Load()
    {
        if (!Managers.Data.Load<JObject>(SaveKey, out var saveData))
        {
            return;
        }

        foreach (var kvp in saveData)
        {
            var quests = kvp.Value as JArray;
            foreach (var token in quests)
            {
                var questSaveData = token.ToObject<QuestSaveData>();
                var newQuest = new Quest(questSaveData);
                NPC.TryRemoveQuest(newQuest.Data.OwnerId, newQuest.Data);

                if (newQuest.State == QuestState.Complete)
                {
                    _completedQuests.Add(newQuest);
                }
                else
                {
                    _activeQuests.Add(newQuest);

                    if (newQuest.State == QuestState.Completable)
                    {
                        NPC.TryAddQuest(newQuest.Data.CompleteOwnerId, newQuest.Data);
                    }
                }
            }
        }
    }

    private JArray CreateQuestsSaveData(List<Quest> quests)
    {
        var saveData = new JArray();

        foreach (var quest in quests)
        {
            var targets = new Dictionary<string, int>();
            foreach (var kvp in quest.Targets)
            {
                targets.Add(kvp.Key.TargetId, kvp.Value);
            }

            var questSaveData = new QuestSaveData()
            {
                QuestId = quest.Data.QuestId,
                State = quest.State,
                Targets = targets,
            };

            saveData.Add(JObject.FromObject(questSaveData));
        }

        return saveData;
    }
}
