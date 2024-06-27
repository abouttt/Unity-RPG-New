using System.Collections.Generic;
using UnityEngine;

public class CooldownManager
{
    private readonly HashSet<Cooldown> _cooldowns = new();
    private readonly Queue<Cooldown> _completedCooldownQueue = new();

    public void Cooling()
    {
        foreach (var cooldown in _cooldowns)
        {
            cooldown.Time -= Time.deltaTime;
            if (cooldown.Time <= 0f)
            {
                cooldown.Time = 0f;
                _completedCooldownQueue.Enqueue(cooldown);
            }
        }

        while (_completedCooldownQueue.Count > 0)
        {
            _cooldowns.Remove(_completedCooldownQueue.Peek());
            _completedCooldownQueue.Dequeue();
        }
    }

    public void AddCooldown(Cooldown cooldown)
    {
        _cooldowns.Add(cooldown);
    }

    public void Clear()
    {
        foreach (var itemData in CooldownDatabase.Instance.CooldownItems)
        {
            (itemData as ICooldownable).Cooldown.Clear();
        }

        foreach (var skillData in CooldownDatabase.Instance.CooldownSkills)
        {
            (skillData as ICooldownable).Cooldown.Clear();
        }

        _cooldowns.Clear();
        _completedCooldownQueue.Clear();
    }
}
