using UnityEngine;

public class ConsumptionItem : StackableItem, IUsableItem, IQuickable
{
    public ConsumptionItemData ConsumptionData { get; private set; }
    public IUsableItemData UsableData => ConsumptionData;

    public ConsumptionItem(ConsumptionItemData data, int count = 1)
        : base(data, count)
    {
        ConsumptionData = data;
    }

    public bool CanUse()
    {
        if (Player.Status.HP <= 0)
        {
            return false;
        }

        if (Player.Status.Level < ConsumptionData.LimitLevel)
        {
            return false;
        }

        if (Count < ConsumptionData.RequiredCount)
        {
            return false;
        }

        if (ConsumptionData.Cooldown.Time > 0f)
        {
            return false;
        }

        SetCount(Count - ConsumptionData.RequiredCount);

        if (IsEmpty)
        {
            Player.ItemInventory.RemoveItem(this);
        }

        ConsumptionData.Cooldown.OnCooldowned();

        return true;
    }

    public bool UseQuick()
    {
        if (!CanUse())
        {
            return false;
        }

        ConsumptionData.Use(Player.QuickInventory, this);

        return true;
    }
}
