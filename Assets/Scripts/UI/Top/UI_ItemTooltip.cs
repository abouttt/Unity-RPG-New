using UnityEngine;
using UnityEngine.UI;

public class UI_ItemTooltip : UI_BaseTooltip
{
    enum Texts
    {
        ItemNameText,
        ItemTypeText,
        ItemDescText,
    }

    [Space(10)]
    [SerializeField]
    private Color _lowColor = Color.white;

    [SerializeField]
    private Color _normalColor = Color.white;

    [SerializeField]
    private Color _highColor = Color.white;

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
    }

    protected override void SetData()
    {
        if (SlotRef.ObjectRef is Item item)
        {
            SetItemData(item.Data);
        }
        else if (SlotRef.ObjectRef is ItemData itemData)
        {
            SetItemData(itemData);
        }
    }

    private void SetItemData(ItemData itemData)
    {
        GetObject((int)GameObjects.Tooltip).SetActive(true);

        if (DataRef != null && DataRef.Equals(itemData))
        {
            return;
        }

        DataRef = itemData;
        GetText((int)Texts.ItemNameText).text = itemData.ItemName;
        SetItemQualityColor(itemData.ItemQuality);
        SetType(itemData.ItemType);
        SetDescription(itemData);
        LayoutRebuilder.ForceRebuildLayoutImmediate(RT);
    }

    private void SetItemQualityColor(ItemQuality itemQuality)
    {
        GetText((int)Texts.ItemNameText).color = itemQuality switch
        {
            ItemQuality.Low => _lowColor,
            ItemQuality.Middle => _normalColor,
            ItemQuality.High => _highColor,
            _ => Color.red,
        };
    }

    private void SetType(ItemType itemType)
    {
        GetText((int)Texts.ItemTypeText).text = itemType switch
        {
            ItemType.Equipment => "[장비 아이템]",
            ItemType.Consumption => "[소비 아이템]",
            ItemType.Etc => "[기타 아이템]",
            _ => "[NULL]"
        };
    }

    private void SetDescription(ItemData itemData)
    {
        SB.Clear();

        if (itemData is IUsableItemData usableData)
        {
            SB.Append($"제한 레벨 : {usableData.LimitLevel} \n");
        }

        if (itemData is EquipmentItemData equipmentData)
        {
            SB.Append("\n");
            AppendValueIfGreaterThan0("공격력", equipmentData.Damage);
            AppendValueIfGreaterThan0("방어력", equipmentData.Defense);
            AppendValueIfGreaterThan0("체력", equipmentData.HP);
            AppendValueIfGreaterThan0("마나", equipmentData.MP);
            AppendValueIfGreaterThan0("기력", equipmentData.SP);
        }
        else if (itemData is ConsumptionItemData consumptionData)
        {
            SB.Append($"소비 개수 : {consumptionData.RequiredCount}\n");
        }

        if (SB.Length > 0)
        {
            SB.Append("\n");
        }

        if (!string.IsNullOrEmpty(itemData.Description))
        {
            SB.Append($"{itemData.Description}\n\n");
        }

        GetText((int)Texts.ItemDescText).text = SB.ToString();
    }

    private void AppendValueIfGreaterThan0(string text, int value)
    {
        if (value > 0)
        {
            SB.Append($"{text} +{value}\n");
        }
    }
}
