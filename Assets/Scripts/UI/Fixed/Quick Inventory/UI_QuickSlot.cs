using UnityEngine;
using UnityEngine.EventSystems;

public class UI_QuickSlot : UI_BaseSlot, IDropHandler
{
    enum Texts
    {
        CountText,
        KeyInfoText,
    }

    enum CooldownImages
    {
        CooldownImage,
    }

    public int Index { get; private set; }

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
        Bind<UI_CooldownImage>(typeof(CooldownImages));
    }

    private void Start()
    {
        Refresh();
    }

    public void Setup(int bindingIndex)
    {
        Index = bindingIndex;
        GetText((int)Texts.KeyInfoText).text = Managers.Input.GetBindingPath("Quick", bindingIndex);
    }

    public void Refresh()
    {
        var quickable = Player.QuickInventory.GetQuickable(Index);

        if (quickable != null)
        {
            if (ObjectRef != quickable)
            {
                if (HasObject)
                {
                    Clear();
                }

                if (quickable is Item item)
                {
                    SetObject(quickable, item.Data.ItemImage);

                    item.Destroyed += OnItemDestroyed;

                    if (item is IStackableItem stackable)
                    {
                        stackable.StackChanged += RefreshCountText;
                    }

                    if (item.Data is ICooldownable cooldownable)
                    {
                        Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).SetCooldown(cooldownable.Cooldown);
                    }
                }
                else if (quickable is Skill skill)
                {
                    SetObject(quickable, skill.Data.SkillImage);

                    skill.SkillChanged += OnSkillLocked;

                    if (skill.Data is ICooldownable cooldownable)
                    {
                        Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).SetCooldown(cooldownable.Cooldown);
                    }
                }
            }

            RefreshCountText();
        }
        else
        {
            Clear();
        }
    }

    protected override void Clear()
    {
        if (ObjectRef is Item item)
        {
            item.Destroyed -= OnItemDestroyed;

            if (item is IStackableItem stackable)
            {
                stackable.StackChanged -= RefreshCountText;
            }

            if (item.Data is ICooldownable)
            {
                Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).Clear();
            }
        }
        else if (ObjectRef is Skill skill)
        {
            skill.SkillChanged -= OnSkillLocked;

            if (skill.Data is ICooldownable cooldownable)
            {
                Get<UI_CooldownImage>((int)CooldownImages.CooldownImage).Clear();
            }
        }

        base.Clear();
        GetText((int)Texts.CountText).gameObject.SetActive(false);
    }

    private void RefreshCountText()
    {
        if (ObjectRef is IStackableItem stackable && stackable.Count > 1)
        {
            GetText((int)Texts.CountText).gameObject.SetActive(true);
            GetText((int)Texts.CountText).text = stackable.Count.ToString();
        }
        else
        {
            GetText((int)Texts.CountText).gameObject.SetActive(false);
        }
    }

    private void OnItemDestroyed()
    {
        Player.QuickInventory.RemoveQuickable(Index);
    }

    private void OnSkillLocked()
    {
        if (ObjectRef is Skill skill && !skill.IsUnlocked)
        {
            Player.QuickInventory.RemoveQuickable(Index);
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

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!CanPointerUp())
        {
            return;
        }

        (ObjectRef as IQuickable).UseQuick();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == gameObject)
        {
            return;
        }

        if (eventData.pointerDrag.TryGetComponent<UI_BaseSlot>(out var otherSlot))
        {
            switch (otherSlot.SlotType)
            {
                case SlotType.Item:
                case SlotType.Skill:
                    if (otherSlot.ObjectRef is not IQuickable quickable)
                    {
                        return;
                    }

                    if (ObjectRef == quickable)
                    {
                        return;
                    }

                    Player.QuickInventory.SetQuickable(quickable, Index);
                    break;
                case SlotType.Quick:
                    Player.QuickInventory.Swap(Index, (otherSlot as UI_QuickSlot).Index);
                    break;
            }
        }
    }
}
