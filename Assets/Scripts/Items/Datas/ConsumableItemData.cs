using UnityEngine;

public abstract class ConsumableItemData : StackableItemData, IUsableItemData, ICooldownable
{
    [field: SerializeField]
    public int LimitLevel { get; private set; } = 1;

    [field: SerializeField]
    public int RequiredCount { get; private set; } = 1;

    [field: SerializeField]
    public Cooldown Cooldown { get; private set; }

    public ConsumableItemData()
    {
        ItemType = ItemType.Consumable;
    }

    public override Item CreateItem()
    {
        return new ConsumableItem(this);
    }

    public override Item CreateItem(int count)
    {
        return new ConsumableItem(this, count);
    }

    public abstract void Use<T>(T inventory, Item item) where T : IInventory;
}
