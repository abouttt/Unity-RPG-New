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

    private ItemData _itemDataRef;

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
    }

    private void OnDisable()
    {
        _itemDataRef = null;
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

        if (_itemDataRef != null && _itemDataRef.Equals(itemData))
        {
            return;
        }

        _itemDataRef = itemData;
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
            ItemType.Equipment => "[��� ������]",
            ItemType.Consumption => "[�Һ� ������]",
            ItemType.Etc => "[��Ÿ ������]",
            _ => "[NULL]"
        };
    }

    private void SetDescription(ItemData itemData)
    {
        SB.Clear();

        if (itemData is IUsableItemData usableData)
        {
            SB.Append($"���� ���� : {usableData.LimitLevel} \n");
        }

        if (itemData is EquipmentItemData equipmentData)
        {
            SB.Append("\n");
            AppendValueIfGreaterThan0("ü��", equipmentData.Stats.HP);
            AppendValueIfGreaterThan0("����", equipmentData.Stats.MP);
            AppendValueIfGreaterThan0("���", (int)equipmentData.Stats.SP);
            AppendValueIfGreaterThan0("���ݷ�", equipmentData.Stats.Damage);
            AppendValueIfGreaterThan0("����", equipmentData.Stats.Defense);
        }
        else if (itemData is ConsumptionItemData consumptionData)
        {
            SB.Append($"�Һ� ���� : {consumptionData.RequiredCount}\n");
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
