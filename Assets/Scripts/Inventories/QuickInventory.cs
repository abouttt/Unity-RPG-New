using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class QuickInventory : MonoBehaviour, IInventory, ISavable
{
    public static string SaveKey => "QuickInventory";

    public event Action<int> InventoryChanged;

    [field: SerializeField]
    public int Capacity { get; private set; }

    private readonly Dictionary<int, IQuickable> _quickables = new();

    private void Awake()
    {
        for (int index = 0; index < Capacity; index++)
        {
            _quickables.Add(index, null);
        }

        Load();
    }

    public void SetQuickable(IQuickable quickable, int index)
    {
        if (quickable == null)
        {
            return;
        }

        _quickables[index] = quickable;
        InventoryChanged?.Invoke(index);
    }

    public void RemoveQuickable(int index)
    {
        if (_quickables[index] == null)
        {
            return;
        }

        _quickables[index] = null;
        InventoryChanged?.Invoke(index);
    }

    public IQuickable GetQuickable(int index)
    {
        return _quickables[index];
    }

    public void Swap(int indexA, int indexB)
    {
        var usableA = _quickables[indexA];
        var usableB = _quickables[indexB];

        if (usableA == null)
        {
            RemoveQuickable(indexB);
        }

        if (usableB == null)
        {
            RemoveQuickable(indexA);
        }

        SetQuickable(usableA, indexB);
        SetQuickable(usableB, indexA);
    }

    public JToken GetSaveData()
    {
        var saveData = new JArray();

        foreach (var kvp in _quickables)
        {
            var quickSaveData = new QuickSaveData
            {
                ItemSaveData = null,
                SkillSaveData = null,
                Index = kvp.Key,
            };

            if (kvp.Value is Item item)
            {
                var itemSaveData = new ItemSaveData
                {
                    ItemId = item.Data.ItemId,
                    Count = 1,
                    Index = Player.ItemInventory.GetItemIndex(item),
                };

                if (item is IStackableItem stackable)
                {
                    itemSaveData.Count = stackable.Count;
                }

                quickSaveData.ItemSaveData = itemSaveData;
            }
            else if (kvp.Value is Skill skill)
            {
                var skillSaveData = new SkillSaveData
                {
                    SkillId = skill.Data.SkillId,
                    Level = -1,
                };

                quickSaveData.SkillSaveData = skillSaveData;
            }

            saveData.Add(JObject.FromObject(quickSaveData));
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
            var quickSaveData = token.ToObject<QuickSaveData>();
            IQuickable quickable = null;

            if (quickSaveData.ItemSaveData.HasValue)
            {
                var itemSaveData = quickSaveData.ItemSaveData.Value;
                var itemData = ItemDatabase.Instance.FindItemById(itemSaveData.ItemId);
                var item = Player.ItemInventory.GetItem<Item>(itemData.ItemType, itemSaveData.Index);
                quickable = item as IQuickable;
            }
            else if (quickSaveData.SkillSaveData.HasValue)
            {
                var skillSaveData = quickSaveData.SkillSaveData.Value;
                var skillData = SkillDatabase.Instance.FindSkillById(skillSaveData.SkillId);
                var skill = Player.SkillTree.GetSkillByData(skillData);
                quickable = skill as IQuickable;
            }

            SetQuickable(quickable, quickSaveData.Index);
        }
    }
}
