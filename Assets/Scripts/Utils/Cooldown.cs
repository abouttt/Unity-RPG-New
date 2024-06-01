using System;
using UnityEngine;

[Serializable]
public class Cooldown
{
    public event Action Cooldowned;

    [field: SerializeField]
    public float MaxTime { get; private set; }
    public float Time;

    public void OnCooldowned()
    {
        Time = MaxTime;
        Managers.Cooldown.AddCooldown(this);
        Cooldowned?.Invoke();
    }

    public void Clear()
    {
        Time = 0f;
        Cooldowned = null;
    }
}
