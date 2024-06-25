using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Active/Kick", fileName = "Skill_Active_Kick")]
public class KickData : ActiveSkillData
{
    public override Skill CreateSkill(int level)
    {
        return new ActiveSkill(this, level);
    }

    public override bool Use(Skill skill)
    {
        if (!base.Use(skill))
        {
            return false;
        }

        Debug.Log("Kick");

        return true;
    }
}
