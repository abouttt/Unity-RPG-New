using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class UIManager
{
    public int ActivePopupCount => _activePopups.Count;
    public bool IsShowedHelperPopup { get; private set; }
    public bool IsShowedSelfishPopup { get; private set; }

    private readonly Dictionary<UIType, Transform> _uiRoots = new();
    private readonly Dictionary<Type, UI_Base> _uiObjects = new();
    private readonly LinkedList<UI_Popup> _activePopups = new();
    private UI_Popup _helperPopup;
    private Transform _root;

    public void Init()
    {
        _root = Util.FindOrInstantiate("UI_Root").transform;

        var types = Enum.GetValues(typeof(UIType));
        for (int i = types.Length - 1; i > 0; i--)
        {
            var type = (UIType)types.GetValue(i);
            _uiRoots.Add(type, new GameObject($"UI_{Enum.GetName(typeof(UIType), (int)type)}_Root").transform);
            _uiRoots[type].SetParent(_root);
        }
    }

    public T Get<T>() where T : UI_Base
    {
        if (_uiObjects.TryGetValue(typeof(T), out var ui))
        {
            return ui as T;
        }

        return null;
    }

    public void Register<T>(UI_Base ui) where T : UI_Base
    {
        if (ui == null)
        {
            Debug.Log($"[UIManager/Register] {typeof(T).Name} is null.");
            return;
        }

        if (ui is not T)
        {
            Debug.Log($"[UIManager/Register] {ui.name} is not {typeof(T)} Type.");
            return;
        }

        if (_uiObjects.ContainsKey(typeof(T)))
        {
            Debug.Log($"[UIManager/Register] {ui.name} is already registered.");
            return;
        }

        if (!ui.TryGetComponent<Canvas>(out var canvas))
        {
            Debug.Log($"[UIManager/Register] {ui.name} is no exist Canvas component.");
            return;
        }
        else
        {
            canvas.sortingOrder = (int)ui.UIType;
        }

        if (ui.UIType == UIType.Popup)
        {
            InitPopup(ui as UI_Popup);
            ui.gameObject.SetActive(false);
        }

        ui.transform.SetParent(_uiRoots[ui.UIType]);
        _uiObjects.Add(typeof(T), ui);
    }

    public void Unregister<T>() where T : UI_Base
    {
        if (_uiObjects.TryGetValue(typeof(T), out var ui))
        {
            if (ui is UI_Popup popup)
            {
                if (popup.IsHelper && IsShowedHelperPopup)
                {
                    IsShowedHelperPopup = false;
                    _helperPopup = null;
                }
                else if (popup.IsSelfish && IsShowedSelfishPopup)
                {
                    IsShowedSelfishPopup = false;
                }

                _activePopups.Remove(popup);
                popup.ClearEvents();
            }

            _uiObjects.Remove(typeof(T));
        }
        else
        {
            Debug.Log($"[UIManager/Unregister] {typeof(T).Name} is no registered.");
        }
    }

    public T Show<T>() where T : UI_Base
    {
        if (_uiObjects.TryGetValue(typeof(T), out var ui))
        {
            if (ui.gameObject.activeSelf)
            {
                return ui as T;
            }

            if (ui.UIType == UIType.Popup)
            {
                var popup = ui as UI_Popup;

                if (IsShowedSelfishPopup && !popup.IgnoreSelfish)
                {
                    return null;
                }

                if (popup.IsHelper)
                {
                    if (_helperPopup != null)
                    {
                        _activePopups.Remove(_helperPopup);
                        _helperPopup.gameObject.SetActive(false);
                    }

                    _helperPopup = popup;
                    IsShowedHelperPopup = true;
                }
                else if (popup.IsSelfish)
                {
                    CloseAll(UIType.Popup);
                    IsShowedSelfishPopup = true;
                }

                _activePopups.AddFirst(popup);
                RefreshAllPopupDepth();
            }

            ui.gameObject.SetActive(true);

            return ui as T;
        }

        return null;
    }

    public bool IsShowed<T>() where T : UI_Base
    {
        if (_uiObjects.TryGetValue(typeof(T), out var ui))
        {
            return ui.gameObject.activeSelf;
        }

        return false;
    }

    public void Close<T>() where T : UI_Base
    {
        if (_uiObjects.TryGetValue(typeof(T), out var ui))
        {
            if (!ui.gameObject.activeSelf)
            {
                return;
            }

            if (ui.UIType == UIType.Popup)
            {
                var popup = ui as UI_Popup;

                if (popup.IsHelper)
                {
                    IsShowedHelperPopup = false;
                    _helperPopup = null;
                }
                else if (popup.IsSelfish)
                {
                    IsShowedSelfishPopup = false;
                }

                _activePopups.Remove(popup);
            }

            ui.gameObject.SetActive(false);
        }
    }

    public void CloseTopPopup()
    {
        if (ActivePopupCount > 0)
        {
            var popup = _activePopups.First.Value;

            if (popup.IsHelper)
            {
                IsShowedHelperPopup = false;
                _helperPopup = null;
            }
            else if (popup.IsSelfish)
            {
                IsShowedSelfishPopup = false;
            }

            _activePopups.RemoveFirst();
            popup.gameObject.SetActive(false);
        }
    }

    public void CloseAll(UIType type)
    {
        if (type == UIType.Popup)
        {
            foreach (var popup in _activePopups)
            {
                popup.gameObject.SetActive(false);
            }

            IsShowedHelperPopup = false;
            IsShowedSelfishPopup = false;
            _activePopups.Clear();
            _helperPopup = null;
        }
        else
        {
            foreach (Transform child in _uiRoots[type])
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void ShowOrClose<T>() where T : UI_Base
    {
        if (IsShowed<T>())
        {
            Close<T>();
        }
        else
        {
            Show<T>();
        }
    }

    public void Clear()
    {
        if (_root != null)
        {
            foreach (Transform child in _root)
            {
                Object.Destroy(child.gameObject);
            }
        }

        IsShowedHelperPopup = false;
        IsShowedSelfishPopup = false;
        _uiRoots.Clear();
        _uiObjects.Clear();
        _activePopups.Clear();
        _helperPopup = null;
    }

    private void InitPopup(UI_Popup popup)
    {
        popup.PopupRT.anchoredPosition = popup.DefaultPosition;

        popup.Focused += () =>
        {
            _activePopups.Remove(popup);
            _activePopups.AddFirst(popup);
            RefreshAllPopupDepth();
        };

        popup.Showed += () =>
        {
            Managers.Input.CursorLocked = false;
        };

        popup.Closed += () =>
        {
            if (_activePopups.Count == 0)
            {
                Managers.Input.CursorLocked = true;
            }
        };
    }

    private void RefreshAllPopupDepth()
    {
        int count = 1;
        foreach (var popup in _activePopups)
        {
            popup.Canvas.sortingOrder = (int)UIType.Top - count++;
        }
    }
}
