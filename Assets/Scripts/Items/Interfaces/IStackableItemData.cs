using UnityEngine;

public interface IStackableItemData
{
    public int MaxCount { get; }

    public Item CreateItem(int count);
}
