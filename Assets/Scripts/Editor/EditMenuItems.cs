#if UNITY_EDITOR
using System.IO;
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

    [MenuItem("Tools/Player/Clear Save Data")]
    public static void ClearSaveData()
    {
        DirectoryInfo directory = new(DataManager.SavePath);
        if (!directory.Exists)
        {
            directory.Create();
        }

        foreach (var file in directory.GetFiles())
        {
            file.Delete();
        }
    }
}
#endif
