using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Active/Kick", fileName = "Skill_Active_Kick")]
public class KickData : ActiveSkillData
{
    public override Skill CreateSkill(int level)
    {
        return new ActiveSkill(this, level);
    }

    public override void Use(Skill skill)
    {
        Debug.Log("Kick");
    }
}
