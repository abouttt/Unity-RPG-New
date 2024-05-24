using System;
using UnityEngine;

public class EtcItem : Item, IStackableItem
{
    public event Action StackChanged;

    public EtcItemData EtcData { get; private set; }
    public IStackableItemData StackableData => EtcData;
    public int MaxCount => EtcData.MaxCount;
    public int Count { get; private set; }
    public bool IsMax => Count >= MaxCount;
    public bool IsEmpty => Count <= 0;

    public EtcItem(EtcItemData data, int count = 1)
        : base(data)
    {
        EtcData = data;
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
