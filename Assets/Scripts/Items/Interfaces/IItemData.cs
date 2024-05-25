using UnityEngine;

public interface IItemData
{
    public string ItemId { get; }
    public string ItemName { get; }
    public Sprite ItemImage { get; }
    public ItemType ItemType { get; }
    public ItemQuality ItemQuality { get; }
    public string Description { get; }

    public Item CreateItem();
    public bool Equals(IItemData other);
}
