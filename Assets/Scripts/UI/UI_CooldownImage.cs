using UnityEngine;
using UnityEngine.UI;

public class UI_CooldownImage : MonoBehaviour
{
    private Cooldown _cooldownRef;
    private Image _cooldownImage;

    private void Awake()
    {
        _cooldownImage = GetComponent<Image>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (_cooldownRef.Time > 0f)
        {
            _cooldownImage.fillAmount = _cooldownRef.Time / _cooldownRef.MaxTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void SetCooldown(Cooldown cooldown)
    {
        if (cooldown == null)
        {
            return;
        }

        _cooldownRef = cooldown;
        _cooldownRef.Cooldowned += Show;
        if (_cooldownRef.Time > 0f)
        {
            gameObject.SetActive(true);
        }
    }

    public void Clear()
    {
        if (_cooldownRef == null)
        {
            return;
        }

        _cooldownRef.Cooldowned -= Show;
        _cooldownRef = null;
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
