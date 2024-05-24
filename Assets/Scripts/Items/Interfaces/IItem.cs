using UnityEngine;

public interface IItem
{
    public string ItemId { get; }
    public string ItemName { get; }
    public Sprite ItemImage { get; }
    public ItemType ItemType { get; }
    public ItemQuality ItemQuality { get; }
    public string Description { get; }

    public Item CreateItem();
}
