using UnityEngine;

public class ConsumptionItem : StackableItem, IUsableItem
{
    public ConsumptionItemData ConsumptionData { get; private set; }
    public override IStackableItemData StackableData => ConsumptionData;
    public IUsableItemData UsableData => ConsumptionData;

    public ConsumptionItem(ConsumptionItemData data, int count = 1)
        : base(data)
    {
        ConsumptionData = data;
        SetCount(count);
    }

    public void Use(int index)
    {
        if (!CanUse())
        {
            return;
        }

        ConsumptionData.Use(index);
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

        if (ConsumptionData.Cooldown.Time > 0f)
        {
            return false;
        }

        if (Count < ConsumptionData.RequiredCount)
        {
            return false;
        }

        int remainingCount = Count - ConsumptionData.RequiredCount;
        if (remainingCount < 0)
        {
            return false;
        }

        SetCount(remainingCount);

        if (IsEmpty)
        {
            Player.ItemInventory.RemoveItem(this);
        }

        ConsumptionData.Cooldown.OnCooldowned();
        Managers.Cooldown.AddCooldown(ConsumptionData.Cooldown);

        return true;
    }
}
