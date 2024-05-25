using System;
using UnityEngine;

public abstract class StackableItem : Item, IStackableItem
{
    public event Action StackChanged;

    public abstract IStackableItemData StackableData { get; }
    public int MaxCount => StackableData.MaxCount;
    public int Count { get; private set; }
    public bool IsMax => Count >= MaxCount;
    public bool IsEmpty => Count <= 0;

    public StackableItem(ItemData data)
        : base(data)
    { }

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
