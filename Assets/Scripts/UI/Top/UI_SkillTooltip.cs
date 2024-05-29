using UnityEngine;
using UnityEngine.UI;

public class UI_SkillTooltip : UI_BaseTooltip
{
    enum Texts
    {
        SkillNameText,
        SkillTypeText,
        SkillDescText,
    }

    private int _prevSkillLevel;

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
    }

    protected override void SetData()
    {
        if (SlotRef.ObjectRef is Skill skill)
        {
            SetSkillData(skill.Data);
        }
        else if (SlotRef.ObjectRef is SkillData skillData)
        {
            SetSkillData(skillData);
        }
    }

    private void SetSkillData(SkillData skillData)
    {
        GetObject((int)GameObjects.Tooltip).SetActive(true);

        if (DataRef != null)
        {
            SetDescription(skillData);

            if (DataRef.Equals(skillData))
            {
                return;
            }
        }

        DataRef = skillData;
        GetText((int)Texts.SkillNameText).text = skillData.SkillName;
        GetText((int)Texts.SkillTypeText).text = $"[{skillData.SkillType}]";
        SetDescription(skillData);
        LayoutRebuilder.ForceRebuildLayoutImmediate(RT);
    }

    private void SetDescription(SkillData skillData)
    {
        SB.Clear();
        SB.Append($"{skillData.Description}\n\n");
        SB.Append("※습득조건※\n");

        var skill = Player.SkillTree.GetSkillByData(skillData);

        foreach (var parent in skill.Parents)
        {
            if (parent.Children.TryGetValue(skill, out var limitLevel))
            {
                SB.Append($"- {parent.Data.SkillName} Lv.{limitLevel}\n");
            }
        }

        SB.Append($"- 필요 스킬 포인트 : {skillData.RequiredSkillPoint}\n");
        SB.Append($"- 제한레벨 : {skillData.LimitLevel}\n\n");

        GetText((int)Texts.SkillDescText).text = SB.ToString();
    }
}
