using UnityEngine;

public class ConsumptionItem : StackableItem
{
    public ConsumptionItemData ConsumptionData { get; private set; }

    public ConsumptionItem(ConsumptionItemData data, int count = 1)
        : base(data, count)
    {
        ConsumptionData = data;
    }

    public void Use()
    {
        if (Player.Status.HP <= 0)
        {
            return;
        }

        if (Player.Status.Level < ConsumptionData.LimitLevel)
        {
            return;
        }

        if (ConsumptionData.Cooldown.Time > 0f)
        {
            return;
        }

        if (Count < ConsumptionData.RequiredCount)
        {
            return;
        }

        int remainingCount = Count - ConsumptionData.RequiredCount;
        if (remainingCount < 0)
        {
            return;
        }

        SetCount(remainingCount);

        if (IsEmpty)
        {
            Player.ItemInventory.RemoveItem(this);
        }

        ConsumptionData.Cooldown.OnCooldowned();
        Managers.Cooldown.AddCooldown(ConsumptionData.Cooldown);

        ConsumptionData.Use();
    }
}
