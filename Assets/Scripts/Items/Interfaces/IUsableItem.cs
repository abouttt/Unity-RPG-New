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

        UsableData.Use(inventory, item);

        return true;
    }

    public bool CanUse();
}
