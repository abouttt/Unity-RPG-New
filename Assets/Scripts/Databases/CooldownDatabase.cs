using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Database/Cooldown Database", fileName = "CooldownDatabase")]
public class CooldownDatabase : SingletonScriptableObject<CooldownDatabase>
{
    public IReadOnlyList<ItemData> CooldownItems => _cooldownItems;
    public IReadOnlyList<SkillData> CooldownSkills => _cooldownSkills;

    [SerializeField]
    private List<ItemData> _cooldownItems;

    [SerializeField]
    private List<SkillData> _cooldownSkills;

#if UNITY_EDITOR
    [ContextMenu("Find Cooldown")]
    public void FindCooldownable()
    {
        FindCooldown<ICooldownable>();
    }

    private void FindCooldown<T>() where T : ICooldownable
    {
        _cooldownItems = new();
        foreach (var itemData in ItemDatabase.Instance.Items)
        {
            if (itemData is ICooldownable)
            {
                _cooldownItems.Add(itemData);
            }
        }

        _cooldownSkills = new();
        foreach (var skillData in SkillDatabase.Instance.Skills)
        {
            if (skillData is ICooldownable)
            {
                _cooldownSkills.Add(skillData);
            }
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
