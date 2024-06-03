using UnityEngine;

public class ConsumableItem : StackableItem, IUsableItem, IQuickable
{
    public ConsumableItemData ConsumableData { get; private set; }
    public IUsableItemData UsableData => ConsumableData;

    public ConsumableItem(ConsumableItemData data, int count = 1)
        : base(data, count)
    {
        ConsumableData = data;
    }

    public bool CanUse()
    {
        if (Player.Status.HP <= 0)
        {
            return false;
        }

        if (Player.Status.Level < ConsumableData.LimitLevel)
        {
            return false;
        }

        if (Count < ConsumableData.RequiredCount)
        {
            return false;
        }

        if (ConsumableData.Cooldown.Time > 0f)
        {
            return false;
        }

        SetCount(Count - ConsumableData.RequiredCount);

        if (IsEmpty)
        {
            Player.ItemInventory.RemoveItem(this);
        }

        ConsumableData.Cooldown.OnCooldowned();

        return true;
    }

    public bool UseQuick()
    {
        if (!CanUse())
        {
            return false;
        }

        ConsumableData.Use(Player.QuickInventory, this);

        return true;
    }
}
