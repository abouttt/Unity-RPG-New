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

    public abstract void Use(Skill skill);
}
