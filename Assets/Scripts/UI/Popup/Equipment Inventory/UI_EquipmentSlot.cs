using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipmentSlot : UI_BaseSlot, IDropHandler
{
    [field: SerializeField]
    public EquipmentType EquipmentType { get; private set; }

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        var item = Player.EquipmentInventory.GetItem(EquipmentType);

        if (item != null)
        {
            SetObject(item, item.Data.ItemImage);
        }
        else
        {
            Clear();
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

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        Managers.UI.Get<UI_EquipmentInventoryPopup>().SetTop();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!CanPointerUp())
        {
            return;
        }

        var equipmentItem = ObjectRef as EquipmentItem;
        equipmentItem.EquipmentData.Use(Player.EquipmentInventory, equipmentItem);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == gameObject)
        {
            return;
        }

        if (eventData.pointerDrag.TryGetComponent<UI_ItemSlot>(out var otherItemSlot))
        {
            if (otherItemSlot.ObjectRef is not EquipmentItem otherItem)
            {
                return;
            }

            if (EquipmentType != otherItem.EquipmentData.EquipmentType)
            {
                return;
            }

            if (otherItem is not IUsableItem otherUsable)
            {
                return;
            }

            otherUsable.Use(Player.ItemInventory, otherItem);
        }
    }
}
