using UnityEngine;

public class ActiveSkill : Skill, IUsableSkill, IQuickable
{
    public ActiveSkillData ActiveData { get; private set; }
    public IUsableSkillData UsableData => ActiveData;

    public ActiveSkill(ActiveSkillData data, int level)
        : base(data, level)
    {
        ActiveData = data;
    }

    public bool CanUse()
    {
        if (ActiveData.Cooldown.Time > 0f)
        {
            return false;
        }

        if (Player.Status.HP < ActiveData.RequiredHP ||
            Player.Status.MP < ActiveData.RequiredMP ||
            Player.Status.SP < 0f)
        {
            return false;
        }

        return true;
    }

    public bool UseQuick()
    {
        if (!CanUse())
        {
            return false;
        }

        return ActiveData.Use(this);
    }
}
