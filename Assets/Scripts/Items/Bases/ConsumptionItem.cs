using System;
using UnityEngine;

public class ConsumptionItem : Item, IStackableItem, IUsableItem
{
    public event Action StackChanged;

    public ConsumptionItemData ConsumptionData { get; private set; }
    public IStackableItemData StackableData => ConsumptionData;
    public IUsableItemData UsableData => ConsumptionData;
    public int MaxCount => ConsumptionData.MaxCount;
    public int Count { get; private set; }
    public bool IsMax => Count >= MaxCount;
    public bool IsEmpty => Count <= 0;

    public ConsumptionItem(ConsumptionItemData data, int count = 1)
        : base(data)
    {
        ConsumptionData = data;
        SetCount(count);
    }

    public void Use()
    {
        if (!CanUse())
        {
            return;
        }

        ConsumptionData.Use();
    }

    public bool CanUse()
    {
        if (Player.Status.HP <= 0)
        {
            return false;
        }

        if (Player.Status.Level < ConsumptionData.LimitLevel)
        {
            return false;
        }

        if (ConsumptionData.Cooldown.Time > 0f)
        {
            return false;
        }

        if (Count < ConsumptionData.RequiredCount)
        {
            return false;
        }

        int remainingCount = Count - ConsumptionData.RequiredCount;
        if (remainingCount < 0)
        {
            return false;
        }

        SetCount(remainingCount);

        if (IsEmpty)
        {
            Player.ItemInventory.RemoveItem(this);
        }

        ConsumptionData.Cooldown.OnCooldowned();
        Managers.Cooldown.AddCooldown(ConsumptionData.Cooldown);

        return true;
    }

    public void SetCount(int count)
    {
        int prevCount = Count;
        Count = Mathf.Clamp(count, 0, MaxCount);
        if (prevCount != Count)
        {
            StackChanged?.Invoke();
        }
    }

    public int AddCountAndGetExcess(int count)
    {
        int nextCount = Count + count;
        SetCount(nextCount);
        return nextCount > MaxCount ? nextCount - MaxCount : 0;
    }
}
