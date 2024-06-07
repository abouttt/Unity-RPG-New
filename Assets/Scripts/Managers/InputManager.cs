using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : GameControls.IPlayerActions
{
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool Sprint { get; private set; }
    public bool Jump { get; private set; }
    public bool LockOn { get; private set; }
    public bool Interaction { get; private set; }

    public bool CursorLocked
    {
        get => _cursorLocked;
        set
        {
            _cursorLocked = value;
            Cursor.visible = !value;
            SetCursorState(value);
        }
    }

    private GameControls _controls;
    private bool _cursorLocked;

    public void Init()
    {
        if (_controls == null)
        {
            _controls = new();
            _controls.Player.SetCallbacks(this);
        }

        _controls.Enable();
    }

    public InputAction GetAction(string actionNameOrId)
    {
        return _controls.FindAction(actionNameOrId);
    }

    public string GetBindingPath(string actionNameOrId, int bindingIndex = 0)
    {
        var key = GetAction(actionNameOrId).bindings[bindingIndex].path;
        return key.GetLastSlashString().ToUpper();
    }

    public void Clear()
    {
        if (_controls == null)
        {
            return;
        }

        _controls.Disable();
        _controls = null;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (CursorLocked)
        {
            Look = context.ReadValue<Vector2>();
        }
        else
        {
            Look = Vector2.zero;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        Sprint = context.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump = context.ReadValueAsButton();
    }

    public void OnLockOn(InputAction.CallbackContext context)
    {
        LockOn = context.ReadValueAsButton();
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        Interaction = context.ReadValueAsButton();
    }

    public void OnCursorToggle(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Managers.UI.IsShowedSelfishPopup)
            {
                return;
            }

            CursorLocked = !_cursorLocked;
        }
    }

    public void OnItemInventory(InputAction.CallbackContext context)
    {
        ShowOrClosePopup<UI_ItemInventoryPopup>(context);
    }

    public void OnEquipmentInventory(InputAction.CallbackContext context)
    {
        ShowOrClosePopup<UI_EquipmentInventoryPopup>(context);
    }

    public void OnQuick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Managers.UI.IsShowedHelperPopup || Managers.UI.IsShowedSelfishPopup)
            {
                return;
            }

            int index = (int)context.ReadValue<float>();
            var quickable = Player.QuickInventory.GetQuickable(index);
            quickable?.UseQuick();
        }
    }

    public void OnQuest(InputAction.CallbackContext context)
    {
        ShowOrClosePopup<UI_QuestPopup>(context);
    }

    public void OnSkillTree(InputAction.CallbackContext context)
    {
        ShowOrClosePopup<UI_SkillTreePopup>(context);
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Managers.UI.ActivePopupCount > 0)
            {
                Managers.UI.CloseTopPopup();
            }
            else
            {
                Managers.UI.Show<UI_GameMenuPopup>();
            }
        }
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void ShowOrClosePopup<T>(InputAction.CallbackContext context) where T : UI_Popup
    {
        if (context.performed)
        {
            if (Managers.UI.IsShowedHelperPopup)
            {
                return;
            }

            Managers.UI.ShowOrClose<T>();
        }
    }
}
