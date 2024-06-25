using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json.Linq;
using AYellowpaper.SerializedCollections;

public class ItemInventory : MonoBehaviour, IInventory, ISavable
{
    [Serializable]
    public class Inventory
    {
        public List<Item> Items;
        public int Capacity;
        [ReadOnly]
        public int Count;
    }

    public static string SaveKey => "SaveItemInventory";

    public event Action<ItemType, int> InventoryChanged;

    public IReadOnlyDictionary<ItemType, Inventory> Inventories => _inventories;

    [SerializeField]
    private SerializedDictionary<ItemType, Inventory> _inventories;

    private readonly Dictionary<Item, int> _itemIndexes = new();

    private void Awake()
    {
        foreach (var kvp in _inventories)
        {
            var nullItems = Enumerable.Repeat<Item>(null, kvp.Value.Capacity);
            kvp.Value.Items = new List<Item>(nullItems);
        }

        Load();
    }

    public int AddItem(ItemData itemData, int count = 1)
    {
        var itemType = itemData.ItemType;
        var items = _inventories[itemType].Items;

        while (count > 0)
        {
            if (itemData is IStackableItemData stackableData)
            {
                int sameItemIndex = items.FindIndex(item =>
                {
                    if (item == null)
                    {
                        return false;
                    }

                    if (!item.Data.Equals(itemData))
                    {
                        return false;
                    }

                    return !(item as IStackableItem).IsMax;
                });

                if (sameItemIndex != -1)
                {
                    int prevCount = count;
                    var otherStackable = items[sameItemIndex] as IStackableItem;
                    count = otherStackable.AddCountAndGetExcess(count);
                    Managers.Quest.ReceiveReport(Category.Item, itemData.ItemId, prevCount - count);
                }
                else
                {
                    if (TryGetEmptyIndex(itemType, out var emptyIndex))
                    {
                        SetItem(itemData, emptyIndex, count);
                        count = Mathf.Max(0, count - stackableData.MaxCount);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (TryGetEmptyIndex(itemType, out var emptyIndex))
                {
                    SetItem(itemData, emptyIndex);
                    count--;
                }
                else
                {
                    break;
                }
            }
        }

        return count;
    }

    public void RemoveItem(Item item)
    {
        if (item == null)
        {
            return;
        }

        if (item.IsDestroyed)
        {
            return;
        }

        var itemType = item.Data.ItemType;
        int index = _itemIndexes[item];
        DestroyItem(itemType, index);
        InventoryChanged?.Invoke(itemType, index);
    }

    public void RemoveItem(ItemType itemType, int index)
    {
        RemoveItem(_inventories[itemType].Items[index]);
    }

    public void RemoveItem(string id, int count)
    {
        if (string.IsNullOrEmpty(id))
        {
            return;
        }

        if (count <= 0)
        {
            return;
        }

        var itemType = GetItemTypeById(id);
        var inventory = _inventories[itemType];

        for (int index = 0; index < inventory.Capacity; index++)
        {
            var item = inventory.Items[index];

            if (item == null)
            {
                continue;
            }

            if (!item.Data.ItemId.Equals(id))
            {
                continue;
            }

            if (item is IStackableItem stackable)
            {
                if (count >= stackable.Count)
                {
                    count -= stackable.Count;
                }
                else
                {
                    stackable.SetCount(stackable.Count - count);
                    break;
                }
            }
            else
            {
                count--;
            }

            RemoveItem(item);

            if (count <= 0)
            {
                break;
            }
        }
    }

    public void SetItem(ItemData itemData, int index, int count = 1)
    {
        if (itemData == null)
        {
            return;
        }

        if (count <= 0)
        {
            return;
        }

        var itemType = itemData.ItemType;
        var inventory = _inventories[itemType];
        var items = inventory.Items;

        if (!IsEmpty(itemType, index))
        {
            DestroyItem(itemType, index);
        }

        items[index] = itemData is IStackableItemData stackableData
            ? stackableData.CreateItem(count)
            : itemData.CreateItem();
        inventory.Count++;
        _itemIndexes.Add(items[index], index);
        Managers.Quest.ReceiveReport(Category.Item, itemData.ItemId, count);
        InventoryChanged?.Invoke(itemType, index);
    }

    public void MoveItem(ItemType itemType, int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex)
        {
            return;
        }

        if (!AddItemCountFromTo(itemType, fromIndex, toIndex))
        {
            SwapItem(itemType, fromIndex, toIndex);
        }
    }

    public void SplitItem(ItemType itemType, int fromIndex, int toIndex, int count)
    {
        if (fromIndex == toIndex)
        {
            return;
        }

        if (count <= 0)
        {
            return;
        }

        if (IsEmpty(itemType, fromIndex) || !IsEmpty(itemType, toIndex))
        {
            return;
        }

        var inventory = _inventories[itemType];

        if (inventory.Items[fromIndex] is not IStackableItem fromItem)
        {
            return;
        }

        int remainingCount = fromItem.Count - count;
        if (remainingCount < 0)
        {
            return;
        }
        else if (remainingCount == 0)
        {
            SwapItem(itemType, fromIndex, toIndex);
        }
        else
        {
            fromItem.SetCount(remainingCount);
            inventory.Items[toIndex] = fromItem.StackableData.CreateItem(count);
            inventory.Count++;
            _itemIndexes.Add(inventory.Items[toIndex], toIndex);
            InventoryChanged?.Invoke(itemType, toIndex);
        }
    }

    public T GetItem<T>(ItemType itemType, int index) where T : Item
    {
        return _inventories[itemType].Items[index] as T;
    }

    public int GetItemIndex(Item item)
    {
        if (_itemIndexes.TryGetValue(item, out var index))
        {
            return index;
        }

        return -1;
    }

    public int GetItemAllCountById(string id)
    {
        var itemType = GetItemTypeById(id);
        int count = 0;

        foreach (var item in _inventories[itemType].Items)
        {
            if (item == null)
            {
                continue;
            }

            if (!item.Data.ItemId.Equals(id))
            {
                continue;
            }

            count += item is IStackableItem stackable ? stackable.Count : 1;
        }

        return count;
    }

    public bool IsEmpty(ItemType itemType, int index)
    {
        return _inventories[itemType].Items[index] == null;
    }

    public JToken GetSaveData()
    {
        var saveData = new JArray();

        foreach (var kvp in _inventories)
        {
            for (int index = 0; index < kvp.Value.Capacity; index++)
            {
                var item = kvp.Value.Items[index];

                if (item == null)
                {
                    continue;
                }

                var itemSaveData = new ItemSaveData
                {
                    ItemId = item.Data.ItemId,
                    Count = item is IStackableItem stackable ? stackable.Count : 1,
                    Index = index,
                };

                saveData.Add(JObject.FromObject(itemSaveData));
            }
        }

        return saveData;
    }

    private bool TryGetEmptyIndex(ItemType itemType, out int index)
    {
        index = _inventories[itemType].Items.FindIndex(item => item == null);
        return index != -1;
    }

    private bool AddItemCountFromTo(ItemType itemType, int fromIndex, int toIndex)
    {
        var items = _inventories[itemType].Items;
        if (items[fromIndex] is not IStackableItem fromItem ||
            items[toIndex] is not IStackableItem toItem)
        {
            return false;
        }

        if (!fromItem.StackableData.Equals(toItem.StackableData))
        {
            return false;
        }

        if (toItem.IsMax)
        {
            return false;
        }

        int excessCount = toItem.AddCountAndGetExcess(fromItem.Count);
        fromItem.SetCount(excessCount);
        if (fromItem.IsEmpty)
        {
            DestroyItem(itemType, fromIndex);
            InventoryChanged?.Invoke(itemType, fromIndex);
        }

        return true;
    }

    private void SwapItem(ItemType itemType, int fromIndex, int toIndex)
    {
        var items = _inventories[itemType].Items;

        if (!IsEmpty(itemType, fromIndex))
        {
            _itemIndexes[items[fromIndex]] = toIndex;
        }

        if (!IsEmpty(itemType, toIndex))
        {
            _itemIndexes[items[toIndex]] = fromIndex;
        }

        (items[fromIndex], items[toIndex]) = (items[toIndex], items[fromIndex]);

        InventoryChanged?.Invoke(itemType, fromIndex);
        InventoryChanged?.Invoke(itemType, toIndex);
    }

    private ItemType GetItemTypeById(string id)
    {
        return id[..id.IndexOf('_')] switch
        {
            "EQUIPMENT" => ItemType.Equipment,
            "CONSUMABLE" => ItemType.Consumable,
            "ETC" => ItemType.Etc,
            _ => throw new NotImplementedException(),
        };
    }

    private void DestroyItem(ItemType itemType, int index)
    {
        var inventory = _inventories[itemType];
        var item = inventory.Items[index];
        int count = item is IStackableItem stackable ? stackable.Count : 1;
        inventory.Items[index] = null;
        inventory.Count--;
        item.Destroy();
        _itemIndexes.Remove(item);
        Managers.Quest.ReceiveReport(Category.Item, item.Data.ItemId, -count);
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
            var itemData = ItemDatabase.Instance.FindItemById(itemSaveData.ItemId);
            var inventory = _inventories[itemData.ItemType];
            var items = inventory.Items;

            items[itemSaveData.Index] = itemData is IStackableItemData stackableData
                ? stackableData.CreateItem(itemSaveData.Count)
                : itemData.CreateItem();
            inventory.Count++;
            _itemIndexes.Add(items[itemSaveData.Index], itemSaveData.Index);
        }
    }
}
