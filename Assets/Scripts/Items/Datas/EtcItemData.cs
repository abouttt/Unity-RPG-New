using UnityEngine;

[CreateAssetMenu(menuName = "Item/Etc", fileName = "Item_Etc_")]
public class EtcItemData : StackableItemData
{
    public EtcItemData()
    {
        ItemType = ItemType.Etc;
    }

    public override Item CreateItem()
    {
        return new EtcItem(this);
    }

    public override Item CreateItem(int count)
    {
        return new EtcItem(this, count);
    }
}
