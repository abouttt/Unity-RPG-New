using UnityEngine;

public interface IUsableItem
{
    public IUsableItemData UsableData { get; }

    public bool Use<T>(T inventory, Item item) where T : IInventory
    {
        if (!CanUse())
        {
            return false;
        }

        return UsableData.Use(inventory, item);
    }

    public bool CanUse();
}
