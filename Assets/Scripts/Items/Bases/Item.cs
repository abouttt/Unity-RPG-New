using UnityEngine;

public abstract class Item
{
    public ItemData Data { get; private set; }

    public Item(ItemData itemData)
    {
        Data = itemData;
    }
}
