using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class ItemInventory : MonoBehaviour, IInventory
{
    [Serializable]
    public class Inventory
    {
        public List<Item> Items;
        public int Capacity;
        [ReadOnly]
        public int Count;
    }

    public event Action<ItemType, int> InventoryChanged;

    public IReadOnlyDictionary<ItemType, Inventory> Inventories => _inventories;

    [SerializeField]
    private SerializedDictionary<ItemType, Inventory> _inventories;
    private readonly Dictionary<Item, int> _itemIndexes = new();

    private void Awake()
    {
        foreach (var kvp in _inventories)
        {
            kvp.Value.Items = Enumerable.Repeat<Item>(null, kvp.Value.Capacity).ToList();
        }
    }

    public int AddItem(ItemData itemData, int count = 1)
    {
        var items = _inventories[itemData.ItemType].Items;

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
                    var otherStackable = items[sameItemIndex] as IStackableItem;
                    count = otherStackable.AddCountAndGetExcess(count);
                }
                else
                {
                    if (TryGetEmptyIndex(itemData.ItemType, out var emptyIndex))
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
                if (TryGetEmptyIndex(itemData.ItemType, out var emptyIndex))
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

        int index = _itemIndexes[item];
        DestroyItem(item.Data.ItemType, index);
        InventoryChanged?.Invoke(item.Data.ItemType, index);
    }

    public void RemoveItem(ItemType itemType, int index)
    {
        RemoveItem(_inventories[itemType].Items[index]);
    }

    public void RemoveItem(string id, int count)
    {
        var itemType = GetItemTypeById(id);

        for (int index = 0; index < _inventories[itemType].Capacity; index++)
        {
            var item = _inventories[itemType].Items[index];

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

            RemoveItem(_inventories[itemType].Items[index]);

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

        var inventory = _inventories[itemData.ItemType];

        if (!IsEmpty(itemData.ItemType, index))
        {
            DestroyItem(itemData.ItemType, index);
        }

        if (itemData is IStackableItemData stackableData)
        {
            inventory.Items[index] = stackableData.CreateItem(count);
        }
        else
        {
            inventory.Items[index] = itemData.CreateItem();
        }

        inventory.Count++;
        _itemIndexes.Add(inventory.Items[index], index);
        InventoryChanged?.Invoke(itemData.ItemType, index);
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

    public int GetSameItemCountById(string id)
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

            if (item is IStackableItem stackable)
            {
                count += stackable.Count;
            }
            else
            {
                count++;
            }
        }

        return count;
    }

    public bool IsEmpty(ItemType itemType, int index)
    {
        return _inventories[itemType].Items[index] == null;
    }

    private bool TryGetEmptyIndex(ItemType itemType, out int index)
    {
        index = _inventories[itemType].Items.FindIndex(item => item == null);
        return index != -1;
    }

    private bool AddItemCountFromTo(ItemType itemType, int fromIndex, int toIndex)
    {
        var inventory = _inventories[itemType];

        if (inventory.Items[fromIndex] is not IStackableItem fromItem ||
            inventory.Items[toIndex] is not IStackableItem toItem)
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
        inventory.Items[index] = null;
        inventory.Count--;
        item.Destroy();
        _itemIndexes.Remove(item);
    }
}
