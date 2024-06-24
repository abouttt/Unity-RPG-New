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

    public void SetNPCShop(NPCShop npcShop)
    {
        foreach (var itemData in npcShop.SaleItems)
        {
            CreateShopSlot(itemData);
        }
    }

    public void BuyItem(UI_ShopSlot slot, int count)
    {
        int price = slot.ItemData.BuyPrice * count;
        if (Player.Status.Gold < price)
        {
            return;
        }

        Player.Status.Gold -= price;
        Player.ItemInventory.AddItem(slot.ItemData, count);
        Managers.UI.Get<UI_ItemInventoryPopup>().ShowItemSlots(slot.ItemData.ItemType);
    }

    public void SellItem(ItemType itemType, int index)
    {
        var item = Player.ItemInventory.GetItem<Item>(itemType, index);
        int count = 1;
        if (item is IStackableItem stackable)
        {
            count = stackable.Count;
        }

        Player.Status.Gold += Mathf.RoundToInt(item.Data.SellPrice * count);
        Player.ItemInventory.RemoveItem(itemType, index);
    }

    private void CreateShopSlot(ItemData itemData)
    {
        var go = Managers.Resource.Instantiate("UI_ShopSlot.prefab", GetObject((int)GameObjects.ShopSlots).transform, true);
        go.GetComponent<UI_ShopSlot>().SetItem(itemData);
        _shopSlots.Add(go);
    }

    private void Clear()
    {
        foreach (var slot in _shopSlots)
        {
            Managers.Resource.Destroy(slot);
        }

        _shopSlots.Clear();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent<UI_ItemSlot>(out var itemSlot))
        {
            SellItem(itemSlot.ItemType, itemSlot.Index);
        }
    }
}
