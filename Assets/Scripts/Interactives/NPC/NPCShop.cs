using System.Collections.Generic;
using UnityEngine;

public class NPCShop : BaseNPCMenu
{
    public IReadOnlyList<ItemData> SaleItems => _saleItems;

    [field: SerializeField]
    private List<ItemData> _saleItems;

    protected override void Awake()
    {
        base.Awake();

        MenuName = "ªÛ¡°";
    }

    public override void Execution()
    {
        Managers.UI.Show<UI_ShopPopup>().SetNPCShop(this);
    }

    public static bool SellItem(ItemType itemType, int index)
    {
        var item = Player.ItemInventory.GetItem<Item>(itemType, index);

        if (item == null)
        {
            return false;
        }

        if (!item.Data.CanSell)
        {
            return false;
        }

        int count = item is IStackableItem stackable ? stackable.Count : 1;
        Player.Status.Gold += Mathf.RoundToInt(item.Data.SellPrice * count);
        Player.ItemInventory.RemoveItem(itemType, index);

        return true;
    }

    public bool BuyItem(int index, int count = 1)
    {
        if (count <= 0)
        {
            return false;
        }

        var item = _saleItems[index];
        int price = item.BuyPrice * count;
        if (Player.Status.Gold < price)
        {
            return false;
        }

        Player.Status.Gold -= price;
        Player.ItemInventory.AddItem(item, count);

        return true;
    }
}
