using System;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public event Action TargetCountChanged;

    public QuestData Data { get; private set; }
    public QuestState State { get; private set; } = QuestState.Inactive;
    public IReadOnlyDictionary<QuestTarget, int> Targets => _targets;

    private readonly Dictionary<QuestTarget, int> _targets = new();

    public Quest(QuestData questData)
    {
        Data = questData;
        State = QuestState.Active;

        foreach (var target in questData.Targets)
        {
            int count = 0;

            switch (target.Category)
            {
                case Category.Item:
                    count = Player.ItemInventory.GetSameItemCountById(target.TargetId);
                    break;
                default:
                    break;
            }

            _targets.Add(target, count);
        }

        CheckCompletable();
    }

    public bool ReceiveReport(Category category, string id, int count)
    {
        if (State == QuestState.Inactive ||
            State == QuestState.Complete)
        {
            return false;
        }

        if (count == 0)
        {
            return false;
        }

        bool isChanged = false;

        foreach (var target in Data.Targets)
        {
            if (target.Category != category)
            {
                continue;
            }

            if (!target.TargetId.Equals(id))
            {
                continue;
            }

            _targets[target] += count;
            isChanged = true;
        }

        if (isChanged)
        {
            CheckCompletable();
            TargetCountChanged?.Invoke();
        }

        return isChanged;
    }

    public bool Complete()
    {
        if (State != QuestState.Completable)
        {
            return false;
        }

        State = QuestState.Complete;
        TargetCountChanged = null;

        Player.Status.Gold += Data.RewardGold;
        Player.Status.XP += Data.RewardXP;

        foreach (var kvp in Data.RewardItems)
        {
            Player.ItemInventory.AddItem(kvp.Key, kvp.Value);
        }

        foreach (var kvp in _targets)
        {
            var target = kvp.Key;
            if (target.Category != Category.Item || !target.RemoveAfterCompletion)
            {
                continue;
            }

            Player.ItemInventory.RemoveItem(target.TargetId, target.CompleteCount);
        }

        return true;
    }

    public void Cancel()
    {
        State = QuestState.Inactive;
        TargetCountChanged = null;
        _targets.Clear();
    }

    public void CheckCompletable()
    {
        if (State == QuestState.Inactive ||
            State == QuestState.Complete)
        {
            return;
        }

        foreach (var kvp in _targets)
        {
            if (kvp.Key.CompleteCount > kvp.Value)
            {
                State = QuestState.Active;
                return;
            }
        }

        State = QuestState.Completable;
    }
}
