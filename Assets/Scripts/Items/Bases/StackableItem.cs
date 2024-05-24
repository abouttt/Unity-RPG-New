using System;
using UnityEngine;

public abstract class StackableItem : Item
{
    public event Action StackChanged;

    public StackableItemData StackableData { get; private set; }
    public int MaxCount => StackableData.MaxCount;
    public int Count { get; private set; }
    public bool IsMax => Count >= MaxCount;
    public bool IsEmpty => Count <= 0;

    public StackableItem(StackableItemData itemData, int count = 1)
        : base(itemData)
    {
        StackableData = itemData;
        SetCount(count);
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
