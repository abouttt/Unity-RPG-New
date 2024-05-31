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
}
