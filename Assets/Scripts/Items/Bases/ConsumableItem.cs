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

        return true;
    }

    public bool UseQuick()
    {
        if (!CanUse())
        {
            return false;
        }

        return ConsumableData.Use(Player.QuickInventory, this);
    }
}
