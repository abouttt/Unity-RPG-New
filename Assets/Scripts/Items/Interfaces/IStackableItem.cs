using System;
using UnityEngine;

public interface IStackableItem
{
    public event Action StackChanged;

    public IStackableItemData StackableData { get; }
    public int MaxCount { get; }
    public int Count { get; }
    public bool IsMax { get; }
    public bool IsEmpty { get; }

    public void SetCount(int count);
    public int AddCountAndGetExcess(int count);
}
