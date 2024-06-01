using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemInventoryPopup : UI_Popup
{
    enum RectTransforms
    {
        EquipmentSlots,
        ConsumptionSlots,
        EtcSlots
    }

    enum Texts
    {
        GoldText
    }

    enum Buttons
    {
        CloseButton,
    }

    enum ScrollRects
    {
        ItemSlotScrollView,
    }

    enum Tabs
    {
        EquipmentTabButton,
        ConsumptionTabButton,
        EtcTabButton,
    }

    [SerializeField]
    private float _tabXOffset;

    [SerializeField]
    private float _tabClickedXPosition;

    private readonly Dictionary<ItemType, UI_ItemSlot[]> _slots = new();
    private readonly Dictionary<UI_ItemInventoryTab, RectTransform> _tabs = new();

    protected override void Init()
    {
        base.Init();

        BindRT(typeof(RectTransforms));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        Bind<ScrollRect>(typeof(ScrollRects));
        Bind<UI_ItemInventoryTab>(typeof(Tabs));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_ItemInventoryPopup>);

        Player.Status.GoldChanged += RefreshGoldText;
        Player.ItemInventory.InventoryChanged += (itemType, index) => _slots[itemType][index].Refresh();

        InitSlots();
        InitTabs();
    }

    private void Start()
    {
        Managers.UI.Register<UI_ItemInventoryPopup>(this);

        Showed += () =>
        {
            PopupRT.SetParent(transform);
            Get<ScrollRect>((int)ScrollRects.ItemSlotScrollView).verticalScrollbar.value = 1f;
        };

        ShowItemSlots(ItemType.Equipment);
        RefreshGoldText();
    }

    public void ShowItemSlots(ItemType itemType)
    {
        var scrollView = Get<ScrollRect>((int)ScrollRects.ItemSlotScrollView);
        scrollView.verticalScrollbar.value = 1f;

        foreach (var kvp in _tabs)
        {
            var pos = kvp.Key.SlotsRT.anchoredPosition;

            if (kvp.Key.TabType == itemType)
            {
                scrollView.content = kvp.Value;
                pos.x = _tabClickedXPosition;
                kvp.Value.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(kvp.Value);
            }
            else
            {
                pos.x = _tabXOffset;
                kvp.Value.gameObject.SetActive(false);
            }

            kvp.Key.SlotsRT.anchoredPosition = pos;
        }
    }

    public void SetActiveCloseButton(bool active)
    {
        GetButton((int)Buttons.CloseButton).gameObject.SetActive(active);
    }

    private void RefreshGoldText()
    {
        GetText((int)Texts.GoldText).text = Player.Status.Gold.ToString();
    }

    private void InitSlots()
    {
        var itemInventory = Player.ItemInventory;
        CreateSlots(itemInventory.Inventories[ItemType.Equipment].Capacity, ItemType.Equipment, GetRT((int)RectTransforms.EquipmentSlots));
        CreateSlots(itemInventory.Inventories[ItemType.Consumption].Capacity, ItemType.Consumption, GetRT((int)RectTransforms.ConsumptionSlots));
        CreateSlots(itemInventory.Inventories[ItemType.Etc].Capacity, ItemType.Etc, GetRT((int)RectTransforms.EtcSlots));

        _slots.Add(ItemType.Equipment, GetRT((int)RectTransforms.EquipmentSlots).GetComponentsInChildren<UI_ItemSlot>());
        _slots.Add(ItemType.Consumption, GetRT((int)RectTransforms.ConsumptionSlots).GetComponentsInChildren<UI_ItemSlot>());
        _slots.Add(ItemType.Etc, GetRT((int)RectTransforms.EtcSlots).GetComponentsInChildren<UI_ItemSlot>());
    }

    private void InitTabs()
    {
        _tabs.Add(Get<UI_ItemInventoryTab>((int)Tabs.EquipmentTabButton), GetRT((int)RectTransforms.EquipmentSlots));
        _tabs.Add(Get<UI_ItemInventoryTab>((int)Tabs.ConsumptionTabButton), GetRT((int)RectTransforms.ConsumptionSlots));
        _tabs.Add(Get<UI_ItemInventoryTab>((int)Tabs.EtcTabButton), GetRT((int)RectTransforms.EtcSlots));

        foreach (var kvp in _tabs)
        {
            kvp.Key.GetComponent<Button>().onClick.AddListener(() =>
            {
                SetTop();
                ShowItemSlots(kvp.Key.TabType);
            });
        }
    }

    private void CreateSlots(int capacity, ItemType itemType, Transform parent)
    {
        for (int i = 0; i < capacity; i++)
        {
            var go = Managers.Resource.Instantiate("UI_ItemSlot.prefab", parent);
            go.GetComponent<UI_ItemSlot>().Setup(itemType, i);
        }
    }
}
