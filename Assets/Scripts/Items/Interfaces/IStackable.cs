using UnityEngine;

public interface IStackable
{
    public int MaxCount { get; }

    public StackableItem CreateItem(int count);
}
