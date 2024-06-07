using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class EquipmentInventory : MonoBehaviour, IInventory, ISavable
{
    public static string SaveKey => "SaveEquipmentInventory";

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

    public JToken GetSaveData()
    {
        var saveData = new JArray();

        foreach (var kvp in _items)
        {
            if (!IsEquipped(kvp.Key))
            {
                continue;
            }

            var itemSaveData = new ItemSaveData()
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
