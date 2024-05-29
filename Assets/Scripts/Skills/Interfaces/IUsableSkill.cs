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

        UsableData.Use(skill);

        return true;
    }

    public bool CanUse();
}
