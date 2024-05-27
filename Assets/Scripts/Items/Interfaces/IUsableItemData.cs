using UnityEngine;

public interface IUsableItemData
{
    public int LimitLevel { get; }

    public void Use<T>(T inventory, Item item) where T : IInventory;
}
