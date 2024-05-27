using UnityEngine;

public class EquipmentItem : Item, IUsableItem
{
    public EquipmentItemData EquipmentData { get; private set; }
    public IUsableItemData UsableData => EquipmentData;

    public EquipmentItem(EquipmentItemData data)
        : base(data)
    {
        EquipmentData = data;
    }

    public bool CanUse()
    {
        if (Player.Status.HP <= 0)
        {
            return false;
        }

        if (Player.Status.Level < EquipmentData.LimitLevel)
        {
            return false;
        }

        return true;
    }
}
