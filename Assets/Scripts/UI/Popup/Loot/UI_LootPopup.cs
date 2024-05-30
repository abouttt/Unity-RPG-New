using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_LootPopup : UI_Popup
{
    enum RectTransforms
    {
        LootSubitems,
    }

    enum Texts
    {
        LootAllText,
    }

    enum Buttons
    {
        CloseButton,
        LootAllButton,
    }

    [SerializeField]
    private float _trackingDistance;

    private FieldItem _fieldItemRef;
    private readonly Dictionary<UI_LootSubitem, ItemData> _lootSubitems = new();
    private bool _isPressedKey;

    protected override void Init()
    {
        base.Init();

        BindRT(typeof(RectTransforms));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetText((int)Texts.LootAllText).text = $"[{Managers.Input.GetBindingPath("Interaction")}] ¸ðµÎ È¹µæ";
        GetButton((int)Buttons.CloseButton).onClick.AddListener(Managers.UI.Close<UI_LootPopup>);
        GetButton((int)Buttons.LootAllButton).onClick.AddListener(AddAllItemToItemInventory);
    }

    private void Start()
    {
        Managers.UI.Register<UI_LootPopup>(this);

        Showed += () => _isPressedKey = Managers.Input.Interaction;
        Closed += Clear;
    }

    private void Update()
    {
        if (_fieldItemRef == null)
        {
            Managers.UI.Close<UI_LootPopup>();
            return;
        }

        TrackingFieldItem();

        if (Managers.Input.Interaction)
        {
            if (!_isPressedKey)
            {
                AddAllItemToItemInventory();
            }
        }
        else
        {
            _isPressedKey = false;
        }
    }

    public void SetFieldItem(FieldItem fieldItem)
    {
        _fieldItemRef = fieldItem;

        foreach (var kvp in fieldItem.Items)
        {
            if (kvp.Key is IStackableItemData stackableData)
            {
                int count = kvp.Value;
                while (count > 0)
                {
                    AddLootSubitem(kvp.Key, Mathf.Clamp(count, count, stackableData.MaxCount));
                    count -= stackableData.MaxCount;
                }
            }
            else
            {
                for (int i = 0; i < kvp.Value; i++)
                {
                    AddLootSubitem(kvp.Key, 1);
                }
            }
        }
    }

    public void AddItemToItemInventory(UI_LootSubitem lootSubitem)
    {
        _fieldItemRef.RemoveItem(lootSubitem.ItemDataRef, lootSubitem.Count);
        int count = Player.ItemInventory.AddItem(lootSubitem.ItemDataRef, lootSubitem.Count);
        if (count > 0)
        {
            lootSubitem.SetItemData(lootSubitem.ItemDataRef, count);
        }
        else
        {
            RemoveLootSubitem(lootSubitem);
        }
    }

    private void AddLootSubitem(ItemData itemData, int count)
    {
        var go = Managers.Resource.Instantiate("UI_LootSubitem.prefab", GetRT((int)RectTransforms.LootSubitems), true);
        var subitem = go.GetComponent<UI_LootSubitem>();
        subitem.SetItemData(itemData, count);
        _lootSubitems.Add(subitem, itemData);
    }

    private void RemoveLootSubitem(UI_LootSubitem lootSubitem)
    {
        _lootSubitems.Remove(lootSubitem);
        Managers.Resource.Destroy(lootSubitem.gameObject);
        if (_lootSubitems.Count == 0)
        {
            Managers.UI.Close<UI_LootPopup>();
        }
    }

    private void TrackingFieldItem()
    {
        if (_trackingDistance < Vector3.Distance(Player.Transform.position, _fieldItemRef.transform.position))
        {
            Managers.UI.Close<UI_LootPopup>();
        }
    }

    private void AddAllItemToItemInventory()
    {
        for (int i = _lootSubitems.Count - 1; i >= 0; i--)
        {
            AddItemToItemInventory(_lootSubitems.ElementAt(i).Key);
        }
    }

    private void Clear()
    {
        foreach (var kvp in _lootSubitems)
        {
            Managers.Resource.Destroy(kvp.Key.gameObject);
        }

        _lootSubitems.Clear();

        if (_fieldItemRef != null)
        {
            _fieldItemRef.IsInteracted = false;
            _fieldItemRef = null;
        }
    }
}
