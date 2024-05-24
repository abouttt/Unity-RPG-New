using UnityEngine;

[CreateAssetMenu(menuName = "Item/Etc", fileName = "Item_Etc_")]
public class EtcItemData : ItemData, IStackableItemData
{
    [field: SerializeField]
    public int MaxCount { get; private set; } = 99;

    public EtcItemData()
    {
        ItemType = ItemType.Etc;
    }

    public override Item CreateItem()
    {
        return new EtcItem(this);
    }

    public Item CreateItem(int count)
    {
        return new EtcItem(this, count);
    }
}
