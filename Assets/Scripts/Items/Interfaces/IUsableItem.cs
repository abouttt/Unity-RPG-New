using UnityEngine;

public interface IUsableItem
{
    public IUsableItemData UsableData { get; }

    public void Use(int index);
    public bool CanUse();
}
