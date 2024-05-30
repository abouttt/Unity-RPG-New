using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class FieldItem : Interactive
{
    public IReadOnlyDictionary<ItemData, int> Items => _items;

    [SerializeField]
    private bool _destroyWhenEmptyItems;

    [SerializeField]
    private SerializedDictionary<ItemData, int> _items;

    private void Start()
    {
        if (_items == null || _items.Count == 0)
        {
            Managers.Resource.Destroy(gameObject);
        }
    }

    public override void Interaction()
    {
        Managers.UI.Show<UI_LootPopup>().SetFieldItem(this);
    }

    public override void Deinteraction()
    {
        throw new System.NotImplementedException();
    }

    public void AddItem(ItemData itemData, int count)
    {
        if (count == 0)
        {
            return;
        }

        if (!_items.ContainsKey(itemData))
        {
            _items.Add(itemData, 0);
        }

        _items[itemData] += count;
    }

    public virtual void RemoveItem(ItemData itemData, int count)
    {
        if (count == 0)
        {
            return;
        }

        if (_items.ContainsKey(itemData))
        {
            _items[itemData] -= count;

            if (_items[itemData] <= 0)
            {
                _items.Remove(itemData);
            }
        }

        if (_items.Count == 0)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");

            if (_destroyWhenEmptyItems)
            {
                Managers.Resource.Destroy(gameObject);
            }
        }
    }
}