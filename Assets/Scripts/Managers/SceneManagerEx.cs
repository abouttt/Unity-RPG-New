using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

public class SceneManagerEx : ISavable
{
    public static string SaveKey => "SaveScene";

    public BaseScene CurrentScene { get { return Object.FindObjectOfType<BaseScene>(); } }
    public SceneType NextScene { get; private set; } = SceneType.Unknown;
    public SceneType PrevScene { get; private set; } = SceneType.Unknown;
    public SceneType SaveScene
    {
        get
        {
            Load();
            return _saveScene;
        }
    }

    private SceneType _saveScene = SceneType.Unknown;

    public void LoadScene(SceneType scene)
    {
        if (CurrentScene is GameScene && Managers.Resource.ResourceCount > 0)
        {
            Managers.Data.Save();
        }

        NextScene = scene;
        PrevScene = CurrentScene.SceneType;
        SceneManager.LoadScene(SceneType.LoadingScene.ToString());
    }

    public JToken GetSaveData()
    {
        return new JArray { Managers.Scene.CurrentScene.SceneType };
    }

    public void Load()
    {
        if (!Managers.Data.Load<JArray>(SaveKey, out var saveData))
        {
            return;
        }

        var sceneSaveData = saveData[0].ToObject<SceneType>();
        _saveScene = sceneSaveData;
    }
}
