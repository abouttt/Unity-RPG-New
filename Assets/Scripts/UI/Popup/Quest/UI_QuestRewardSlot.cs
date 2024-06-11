using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_QuestRewardSlot : UI_BaseSlot
{
    enum Texts
    {
        RewardNameText,
        RewardCountText,
    }

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
    }

    public void SetReward(ItemData itemData, int count)
    {
        SetObject(itemData, itemData.ItemImage);
        GetText((int)Texts.RewardNameText).text = itemData.ItemName;
        GetText((int)Texts.RewardCountText).text = $"x{count}";
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        eventData.Use();
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
