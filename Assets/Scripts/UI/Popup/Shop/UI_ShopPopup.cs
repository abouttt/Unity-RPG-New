using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ShopPopup : UI_Popup, IDropHandler
{
    enum GameObjects
    {
        ShopSlots,
    }

    enum Buttons
    {
        CloseButton,
    }

    [SerializeField]
    private Vector3 _itemInventoryPos;

    private Vector3 _prevItemInventoryPos;

    private NPCShop _shopRef;
    private readonly List<GameObject> _shopSlots = new();

    protected override void Init()
    {
        base.Init();

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_ShopPopup>);
    }

    private void Start()
    {
        Managers.UI.Register<UI_ShopPopup>(this);

        Showed += () =>
        {
            var itemInventory = Managers.UI.Get<UI_ItemInventoryPopup>();
            _prevItemInventoryPos = itemInventory.PopupRT.anchoredPosition;
            itemInventory.PopupRT.anchoredPosition = _itemInventoryPos;
            itemInventory.PopupRT.SetParent(transform);
            itemInventory.SetActiveCloseButton(false);
            Managers.UI.Get<UI_NPCMenuPopup>().PopupRT.gameObject.SetActive(false);
        };

        Closed += () =>
        {
            Clear();

            var itemInventory = Managers.UI.Get<UI_ItemInventoryPopup>();
            itemInventory.PopupRT.anchoredPosition = _prevItemInventoryPos;
            itemInventory.SetActiveCloseButton(true);
            Managers.UI.Get<UI_NPCMenuPopup>().PopupRT.gameObject.SetActive(true);
        };
    }

    public void SetNPCShop(NPCShop shop)
    {
        _shopRef = shop;

        for (int index = 0; index < shop.SaleItems.Count; index++)
        {
            CreateShopSlot(shop.SaleItems[index], index);
        }
    }

    public void BuyItem(UI_ShopSlot slot, int count)
    {
        if (_shopRef.BuyItem(slot.Index, count))
        {
            Managers.UI.Get<UI_ItemInventoryPopup>().ShowItemSlots(slot.ItemData.ItemType);
        }
    }

    private void CreateShopSlot(ItemData itemData, int index)
    {
        var go = Managers.Resource.Instantiate("UI_ShopSlot.prefab", GetObject((int)GameObjects.ShopSlots).transform, true);
        go.GetComponent<UI_ShopSlot>().SetItem(itemData, index);
        _shopSlots.Add(go);
    }

    private void Clear()
    {
        foreach (var slot in _shopSlots)
        {
            Managers.Resource.Destroy(slot);
        }

        _shopRef = null;
        _shopSlots.Clear();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent<UI_ItemSlot>(out var itemSlot))
        {
            NPCShop.SellItem(itemSlot.ItemType, itemSlot.Index);
        }
    }
}
