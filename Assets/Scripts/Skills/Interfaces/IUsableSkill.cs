using UnityEngine;

public interface IUsableSkill
{
    public IUsableSkillData UsableData { get; }

    public bool Use(Skill skill)
    {
        if (!CanUse())
        {
            return false;
        }

        return UsableData.Use(skill);
    }

    public bool CanUse();
}
