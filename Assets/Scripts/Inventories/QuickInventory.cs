using System;
using System.Collections.Generic;
using UnityEngine;

public class QuickInventory : MonoBehaviour, IInventory
{
    public event Action<int> InventoryChanged;

    [field: SerializeField]
    public int Capacity { get; private set; }

    private readonly Dictionary<int, IQuickable> _quickables = new();

    private void Awake()
    {
        for (int i = 0; i < Capacity; i++)
        {
            _quickables.Add(i, null);
        }
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
}
