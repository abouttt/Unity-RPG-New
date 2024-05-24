using UnityEngine;

public class EquipmentItem : Item
{
    public EquipmentItemData EquipmentData { get; private set; }

    public EquipmentItem(EquipmentItemData data)
        : base(data)
    {
        EquipmentData = data;
    }

    public void Use()
    {
        if (Player.Status.HP <= 0)
        {
            return;
        }

        if (Player.Status.Level < EquipmentData.LimitLevel)
        {
            return;
        }

        EquipmentData.Use();
    }
}
