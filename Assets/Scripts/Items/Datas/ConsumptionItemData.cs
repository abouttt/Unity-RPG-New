using UnityEngine;

public abstract class ConsumptionItemData : ItemData, IStackableItemData, IUsableItemData, ICooldownable
{
    [field: SerializeField]
    public int MaxCount { get; private set; } = 99;

    [field: SerializeField]
    public int LimitLevel { get; private set; } = 1;

    [field: SerializeField]
    public int RequiredCount { get; private set; } = 1;

    [field: SerializeField]
    public Cooldown Cooldown { get; private set; }

    public ConsumptionItemData()
    {
        ItemType = ItemType.Consumption;
    }

    public override Item CreateItem()
    {
        return new ConsumptionItem(this);
    }

    public Item CreateItem(int count)
    {
        return new ConsumptionItem(this, count);
    }

    public abstract void Use();
}
