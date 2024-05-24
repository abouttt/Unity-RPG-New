using UnityEngine;

public interface IUsableItem
{
    public IUsableItemData UsableData { get; }

    public void Use();
    public bool CanUse();
}
