using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Active/Punch", fileName = "Skill_Active_Punch")]
public class PunchData : ActiveSkillData
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

        Debug.Log("Punch");

        return true;
    }
}
