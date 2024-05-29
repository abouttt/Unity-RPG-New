using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    [SerializeField]
    private SkillData[] _skillDatas;
    private readonly List<Skill> _rootSkills = new();
    private readonly Dictionary<SkillData, Skill> _skills = new();

    private void Awake()
    {
        // 스킬 생성 및 루트 스킬 설정
        foreach (var skillData in _skillDatas)
        {
            var skill = skillData.CreateSkill(0);
            _skills.Add(skillData, skill);
            if (skillData.Root)
            {
                _rootSkills.Add(skill);
            }
        }

        // 자식 스킬 설정
        foreach (var skillData in _skillDatas)
        {
            foreach (var kvp in skillData.Children)
            {
                _skills[skillData].AddChild(_skills[kvp.Key], kvp.Value);
            }
        }

        Player.Status.SkillPointChanged += CheckRootSkills;
    }

    private void Start()
    {
        CheckRootSkills();
    }

    public void CheckRootSkills()
    {
        foreach (var skill in _rootSkills)
        {
            skill.CheckState();
        }
    }

    public Skill GetSkillByData(SkillData skillData)
    {
        if (_skills.TryGetValue(skillData, out var skill))
        {
            return skill;
        }

        return null;
    }

    public void ResetAllSkill()
    {
        int totalSkillPoint = 0;
        foreach (var skill in _rootSkills)
        {
            totalSkillPoint += skill.ResetSkill();
        }

        Player.Status.SkillPoint += totalSkillPoint;
    }
}
