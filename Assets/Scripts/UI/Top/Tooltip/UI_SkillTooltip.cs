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

    private SkillData _skillDataRef;

    protected override void Init()
    {
        base.Init();

        BindText(typeof(Texts));
    }

    private void OnDisable()
    {
        _skillDataRef = null;
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

        if (_skillDataRef != null && _skillDataRef.Equals(skillData))
        {
            return;
        }

        _skillDataRef = skillData;
        GetText((int)Texts.SkillNameText).text = skillData.SkillName;
        GetText((int)Texts.SkillTypeText).text = $"[{skillData.SkillType}]";
        SetDescription(skillData);
        LayoutRebuilder.ForceRebuildLayoutImmediate(RT);
    }

    private void SetDescription(SkillData skillData)
    {
        SB.Clear();
        SB.Append($"{skillData.Description}\n\n");
        SB.Append("�ؽ������ǡ�\n");

        var skill = Player.SkillTree.GetSkill(skillData);

        foreach (var parent in skill.Parents)
        {
            if (parent.Children.TryGetValue(skill, out var limitLevel))
            {
                SB.Append($"- {parent.Data.SkillName} Lv.{limitLevel}\n");
            }
        }

        SB.Append($"- �ʿ� ��ų ����Ʈ : {skillData.RequiredSkillPoint}\n");
        SB.Append($"- ���ѷ��� : {skillData.LimitLevel}\n\n");

        GetText((int)Texts.SkillDescText).text = SB.ToString();
    }
}
