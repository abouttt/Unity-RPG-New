using UnityEngine;

public abstract class ActiveSkillData : SkillData, IUsableSkillData, ICooldownable
{
    [field: SerializeField]
    public int RequiredHP { get; private set; }

    [field: SerializeField]
    public int RequiredMP { get; private set; }

    [field: SerializeField]
    public int RequiredSP { get; private set; }

    [field: SerializeField]
    public Cooldown Cooldown { get; private set; }

    public ActiveSkillData()
    {
        SkillType = SkillType.Active;
    }

    public virtual bool Use(Skill skill)
    {
        if (skill == null)
        {
            return false;
        }

        if (skill.Data != this)
        {
            return false;
        }

        Player.Status.HP -= RequiredHP;
        Player.Status.MP -= RequiredMP;
        Player.Status.SP -= RequiredSP;

        Cooldown.OnCooldowned();

        return true;
    }
}
