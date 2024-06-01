using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : MonoBehaviour, IInventory
{
    public event Action<EquipmentType> InventoryChanged;

    private readonly Dictionary<EquipmentType, EquipmentItem> _items = new();
    private BasicStats _stats = new();

    private void Awake()
    {
        var types = Enum.GetValues(typeof(EquipmentType));
        for (int i = 0; i < types.Length; i++)
        {
            _items.Add((EquipmentType)types.GetValue(i), null);
        }
    }

    public void Equip(EquipmentItemData equipmentItemData)
    {
        var equipmentType = equipmentItemData.EquipmentType;
        if (IsEquipped(equipmentType))
        {
            Unequip(equipmentType);
        }

        var equipmentItem = equipmentItemData.CreateItem() as EquipmentItem;
        _items[equipmentType] = equipmentItem;
        _stats += equipmentItemData.Stats;
        InventoryChanged?.Invoke(equipmentType);
    }

    public void Unequip(EquipmentType equipmentType)
    {
        if (!IsEquipped(equipmentType))
        {
            return;
        }

        _stats -= _items[equipmentType].EquipmentData.Stats;
        _items[equipmentType] = null;
        InventoryChanged?.Invoke(equipmentType);
    }

    public EquipmentItem GetItem(EquipmentType equipmentType)
    {
        return _items[equipmentType];
    }

    public bool IsEquipped(EquipmentType equipmentType)
    {
        return _items[equipmentType] != null;
    }

    public BasicStats GetStats()
    {
        return new BasicStats
        {
            HP = _stats.HP,
            MP = _stats.MP,
            SP = _stats.SP,
            Damage = _stats.Damage,
            Defense = _stats.Defense,
        };
    }
}
