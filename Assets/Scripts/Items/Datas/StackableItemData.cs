using UnityEngine;

public abstract class StackableItemData : ItemData, IStackable
{
    [field: SerializeField]
    public int MaxCount { get; private set; } = 99;

    public abstract StackableItem CreateItem(int count);
}
