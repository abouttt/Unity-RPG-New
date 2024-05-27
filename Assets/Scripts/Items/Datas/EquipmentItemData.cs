using UnityEngine;

[CreateAssetMenu(menuName = "Item/Equipment", fileName = "Item_Equipment_")]
public class EquipmentItemData : ItemData, IUsableItemData
{
    [field: SerializeField]
    public int LimitLevel { get; private set; } = 1;

    [field: SerializeField]
    public EquipmentType EquipmentType { get; private set; }

    [field: SerializeField]
    public GameObject EquipmentPrefab { get; private set; }

    [field: SerializeField]
    public int HP { get; private set; }

    [field: SerializeField]
    public int MP { get; private set; }

    [field: SerializeField]
    public int SP { get; private set; }

    [field: SerializeField]
    public int Damage { get; private set; }

    [field: SerializeField]
    public int Defense { get; private set; }

    public EquipmentItemData()
    {
        ItemType = ItemType.Equipment;
    }

    public override Item CreateItem()
    {
        return new EquipmentItem(this);
    }

    public void Use(int index)
    {
        var equippedItem = Player.EquipmentInventory.GetItem(EquipmentType);
        if (equippedItem != null)
        {
            Player.ItemInventory.SetItem(equippedItem.EquipmentData, index);
        }
        else
        {
            Player.ItemInventory.RemoveItem(ItemType, index);
        }

        Player.EquipmentInventory.Equip(this);
    }
}
