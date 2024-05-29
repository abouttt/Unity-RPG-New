using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Passive/BodyStrengthening", fileName = "Skill_Passive_BodyStrengthening")]
public class BodyStrengtheningData : PassiveSkillData
{
    public override Skill CreateSkill(int level)
    {
        return new PassiveSkill(this, level);
    }
}
