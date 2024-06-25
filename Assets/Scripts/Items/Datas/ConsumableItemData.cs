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

    public virtual bool Use<T>(T inventory, Item item) where T : IInventory
    {
        if (item == null)
        {
            return false;
        }

        if (item.Data != this)
        {
            return false;
        }

        var consumable = item as ConsumableItem;
        consumable.SetCount(consumable.Count - RequiredCount);
        if (consumable.IsEmpty)
        {
            Player.ItemInventory.RemoveItem(item);
        }

        Cooldown.OnCooldowned();
        Managers.Quest.ReceiveReport(Category.Item, ItemId, -RequiredCount);

        return true;
    }
}
