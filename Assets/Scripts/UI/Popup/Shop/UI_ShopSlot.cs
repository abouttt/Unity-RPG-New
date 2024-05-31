using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ShopSlot : UI_BaseSlot, IPointerEnterHandler, IPointerExitHandler
{
    enum Texts
    {
        ItemNameText,
        PriceText,
    }

    public ItemData ItemData => ObjectRef as ItemData;

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
    }

    public void SetItem(ItemData itemData)
    {
        SetObject(itemData, itemData.ItemImage);
        GetText((int)Texts.ItemNameText).text = itemData.ItemName;
        GetText((int)Texts.PriceText).text = itemData.Price.ToString();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!CanPointerUp())
        {
            return;
        }

        if (Managers.Input.Sprint && ItemData is IStackableItemData stackableData)
        {
            var splitPopup = Managers.UI.Show<UI_ItemSplitPopup>();
            splitPopup.SetEvent(() =>
                Managers.UI.Get<UI_ShopPopup>().BuyItem(this, splitPopup.Count),
                $"[{ItemData.ItemName}] 구매수량", 1, stackableData.MaxCount, ItemData.Price, true);
        }
        else
        {
            Managers.UI.Get<UI_ShopPopup>().BuyItem(this, 1);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        Managers.UI.Get<UI_TooltipTop>().ItemTooltip.SetSlot(this);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        Managers.UI.Get<UI_TooltipTop>().ItemTooltip.SetSlot(null);
    }
}
