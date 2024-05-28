using System;
using UnityEngine;

public abstract class Item
{
    public event Action Destroyed;

    public ItemData Data { get; private set; }
    public bool IsDestroyed { get; private set; }

    public Item(ItemData itemData)
    {
        Data = itemData;
    }

    public void Destroy()
    {
        if (IsDestroyed)
        {
            return;
        }

        IsDestroyed = true;
        Destroyed?.Invoke();
        Destroyed = null;
    }
}
