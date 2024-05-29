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

    public void SubRequired()
    {
        Player.Status.HP -= ActiveData.RequiredHP;
        Player.Status.MP -= ActiveData.RequiredMP;
        Player.Status.SP -= ActiveData.RequiredSP;

        ActiveData.Cooldown.OnCooldowned();
        Managers.Cooldown.AddCooldown(ActiveData.Cooldown);
    }

    public bool CanUse()
    {
        if (ActiveData.Cooldown.Time > 0f)
        {
            return false;
        }

        if (Player.Status.HP < 0 ||
            Player.Status.MP < 0 ||
            Player.Status.SP < 0f)
        {
            return false;
        }

        SubRequired();

        return true;
    }

    public bool UseQuick()
    {
        if (!CanUse())
        {
            return false;
        }

        ActiveData.Use(this);

        return true;
    }
}
