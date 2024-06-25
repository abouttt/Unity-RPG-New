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
    public BasicStats FixedStats { get; private set; }

    [field: SerializeField]
    public BasicStats PercentageStats { get; private set; }

    public EquipmentItemData()
    {
        ItemType = ItemType.Equipment;
    }

    public override Item CreateItem()
    {
        return new EquipmentItem(this);
    }

    public bool Use<T>(T inventory, Item item) where T : IInventory
    {
        if (item == null)
        {
            return false;
        }

        if (item.Data != this)
        {
            return false;
        }

        switch (inventory)
        {
            case ItemInventory:
                var equippedItem = Player.EquipmentInventory.GetItem(EquipmentType);
                var index = Player.ItemInventory.GetItemIndex(item);
                if (equippedItem != null)
                {
                    Player.ItemInventory.SetItem(equippedItem.EquipmentData, index);
                }
                else
                {
                    Player.ItemInventory.RemoveItem(ItemType, index);
                }
                Player.EquipmentInventory.Equip(this);
                break;
            case EquipmentInventory:
                Player.ItemInventory.AddItem(this);
                Player.EquipmentInventory.Unequip(EquipmentType);
                break;
            default:
                return false;
        }

        return true;
    }
}
