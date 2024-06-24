using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class EquipmentInventory : MonoBehaviour, IInventory, ISavable
{
    public static string SaveKey => "SaveEquipmentInventory";

    public event Action<EquipmentType> InventoryChanged;

    private readonly Dictionary<EquipmentType, EquipmentItem> _items = new();
    private BasicStats _fixedStats = new();
    private BasicStats _percentageStats = new();

    private void Awake()
    {
        var types = Enum.GetValues(typeof(EquipmentType));
        foreach (EquipmentType type in types)
        {
            _items.Add(type, null);
        }

        Load();
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
        _fixedStats += equipmentItemData.FixedStats;
        _percentageStats += equipmentItemData.PercentageStats;
        InventoryChanged?.Invoke(equipmentType);
    }

    public void Unequip(EquipmentType equipmentType)
    {
        if (!IsEquipped(equipmentType))
        {
            return;
        }

        _fixedStats -= _items[equipmentType].EquipmentData.FixedStats;
        _percentageStats -= _items[equipmentType].EquipmentData.PercentageStats;
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

    public BasicStats GetFixedStats()
    {
        return new BasicStats
        {
            HP = _fixedStats.HP,
            MP = _fixedStats.MP,
            SP = _fixedStats.SP,
            Damage = _fixedStats.Damage,
            Defense = _fixedStats.Defense,
        };
    }

    public BasicStats GetPercentageStats()
    {
        return new BasicStats
        {
            HP = _percentageStats.HP,
            MP = _percentageStats.MP,
            SP = _percentageStats.SP,
            Damage = _percentageStats.Damage,
            Defense = _percentageStats.Defense,
        };
    }

    public JToken GetSaveData()
    {
        var saveData = new JArray();

        foreach (var kvp in _items)
        {
            if (!IsEquipped(kvp.Key))
            {
                continue;
            }

            var itemSaveData = new ItemSaveData
            {
                ItemId = kvp.Value.Data.ItemId,
            };

            saveData.Add(JObject.FromObject(itemSaveData));
        }

        return saveData;
    }

    private void Load()
    {
        if (!Managers.Data.Load<JArray>(SaveKey, out var saveData))
        {
            return;
        }

        foreach (var token in saveData)
        {
            var itemSaveData = token.ToObject<ItemSaveData>();
            Equip(ItemDatabase.Instance.FindItemById(itemSaveData.ItemId) as EquipmentItemData);
        }
    }
}
