using UnityEngine;

public abstract class StackableItemData : ItemData, IStackableItemData
{
    [field: SerializeField]
    public int MaxCount { get; private set; } = 99;

    public abstract Item CreateItem(int count);
}
