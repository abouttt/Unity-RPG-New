using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : GameControls.IPlayerActions
{
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
        _controls.Disable();
        _controls.Dispose();
        _controls = null;
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
