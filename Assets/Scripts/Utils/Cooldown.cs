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
        MaxTime = Time;
        Cooldowned?.Invoke();
    }

    public void Clear()
    {
        Time = 0;
        Cooldowned = null;
    }
}
