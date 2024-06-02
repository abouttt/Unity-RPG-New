#if UNITY_EDITOR
using UnityEditor;

public static class EditMenuItems
{
    [MenuItem("Tools/Player/Refresh Databases")]
    public static void RefreshDatabases()
    {
        ItemDatabase.Instance.FindItems();
        SkillDatabase.Instance.FindSkills();
        CooldownDatabase.Instance.FindCooldownable();
        QuestDatabase.Instance.FindQuests();
    }
}
#endif
