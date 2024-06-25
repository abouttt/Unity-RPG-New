using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class SkillTree : MonoBehaviour, ISavable
{
    public static string SaveKey => "SaveSkillTree";

    [SerializeField]
    private SkillData[] _skillDatas;
    private readonly List<Skill> _rootSkills = new();
    private readonly Dictionary<SkillData, Skill> _skills = new();
    private readonly Dictionary<SkillData, int> _saveSkillLevels = new();

    private void Awake()
    {
        Load();

        // 스킬 생성 및 루트 스킬 설정
        foreach (var skillData in _skillDatas)
        {
            _saveSkillLevels.TryGetValue(skillData, out int level);
            var skill = skillData.CreateSkill(level);
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

    public Skill GetSkill(SkillData skillData)
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

    public JToken GetSaveData()
    {
        var saveData = new JArray();

        foreach (var kvp in _skills)
        {
            var skillSaveData = new SkillSaveData()
            {
                SkillId = kvp.Key.SkillId,
                Level = kvp.Value.Level,
            };

            saveData.Add(JObject.FromObject(skillSaveData));
        }

        return saveData;
    }

    private void Load()
    {
        if (!Managers.Data.Load<JArray>(SaveKey, out var saveData))
        {
            return;
        }

        foreach (var token in saveData)
        {
            var skillSaveData = token.ToObject<SkillSaveData>();
            if (skillSaveData.Level == 0)
            {
                continue;
            }

            var skillData = SkillDatabase.Instance.FindSkillById(skillSaveData.SkillId);
            _saveSkillLevels.Add(skillData, skillSaveData.Level);
        }
    }
}
