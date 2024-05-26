using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class UI_BaseTooltip : UI_Base
{
    protected enum GameObjects
    {
        Tooltip,
    }

    protected UI_BaseSlot SlotRef;
    protected IItemData DataRef;
    protected RectTransform RT;
    protected readonly StringBuilder SB = new(50);

    protected override void Init()
    {
        BindObject(typeof(GameObjects));
        RT = GetObject((int)GameObjects.Tooltip).GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        GetObject((int)GameObjects.Tooltip).SetActive(false);
    }

    private void Update()
    {
        if (SlotRef == null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!SlotRef.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            return;
        }

        if (UI_BaseSlot.IsDragging)
        {
            GetObject((int)GameObjects.Tooltip).SetActive(false);
            return;
        }

        if (SlotRef.HasObject)
        {
            SetData();
        }
        else
        {
            GetObject((int)GameObjects.Tooltip).SetActive(false);
        }

        SetPosition(Mouse.current.position.ReadValue());
    }

    public void SetSlot(UI_BaseSlot slot)
    {
        SlotRef = slot;
        gameObject.SetActive(slot != null);
    }

    protected abstract void SetData();

    private void SetPosition(Vector3 position)
    {
        RT.position = new Vector3()
        {
            x = position.x + (RT.rect.width * 0.5f),
            y = position.y + (RT.rect.height * 0.5f)
        };
    }
}
