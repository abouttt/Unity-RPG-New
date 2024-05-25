using UnityEngine;

public abstract class Item : IItem
{
    public IItemData Data { get; private set; }

    public Item(ItemData itemData)
    {
        Data = itemData;
    }
}
