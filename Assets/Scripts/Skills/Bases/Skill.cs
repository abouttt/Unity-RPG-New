using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class Skill
{
    public event Action SkillChanged;

    public SkillData Data { get; private set; }
    public bool IsAcquirable { get; private set; }
    public bool IsUnlocked { get; private set; }
    public int Level { get; private set; }
    public IReadOnlyList<Skill> Parents => _parents;
    public IReadOnlyDictionary<Skill, int> Children => _children;

    protected StringBuilder SB { get; private set; } = new(50);

    private readonly List<Skill> _parents = new();
    private readonly Dictionary<Skill, int> _children = new();

    public Skill(SkillData data, int level)
    {
        Data = data;
        Level = level;
        if (level > 0)
        {
            IsAcquirable = true;
            IsUnlocked = true;
        }
    }

    public void CheckState()
    {
        if (IsUnlocked)
        {
            foreach (var kvp in Children)
            {
                kvp.Key.CheckState();
            }
        }

        if (Level == Data.MaxLevel)
        {
            return;
        }

        if (!IsAcquirable)
        {
            if (!CheckParentsLevel())
            {
                return;
            }
        }

        if (Player.Status.Level >= Data.LimitLevel)
        {
            if (!IsAcquirable)
            {
                IsAcquirable = true;
            }

            SkillChanged?.Invoke();
        }
    }

    public virtual void LevelUp()
    {
        if (Level == Data.MaxLevel)
        {
            return;
        }

        if (!IsAcquirable)
        {
            return;
        }

        if (!IsUnlocked)
        {
            IsUnlocked = true;
        }

        Level++;
        Player.Status.SkillPoint -= Data.RequiredSkillPoint;
        SkillChanged?.Invoke();
    }

    public virtual int ResetSkill()
    {
        int skillPoint = Level;

        if (IsUnlocked)
        {
            foreach (var kvp in _children)
            {
                skillPoint += kvp.Key.ResetSkill();
            }
        }

        var prevAcquirable = IsAcquirable;
        var prevUnlocked = IsUnlocked;

        Level = 0;
        IsAcquirable = false;
        IsUnlocked = false;

        if (prevAcquirable)
        {
            SkillChanged?.Invoke();
        }

        if (prevUnlocked)
        {

        }

        return skillPoint;
    }

    public void AddChild(Skill skill, int limitLevel)
    {
        if (_children.TryGetValue(skill, out _))
        {
            return;
        }

        _children.Add(skill, limitLevel);
        skill._parents.Add(this);
    }

    private bool CheckParentsLevel()
    {
        foreach (var parents in _parents)
        {
            if (parents.Level < parents._children[this])
            {
                return false;
            }
        }

        return true;
    }
}
