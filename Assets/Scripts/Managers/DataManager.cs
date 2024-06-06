using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class DataManager
{
    public static readonly string SavePath = $"{Application.streamingAssetsPath}/Saved";
    public static readonly string SaveFilePath = $"{SavePath}/Saves.json";
    public static readonly string SaveMetaFilePath = $"{SavePath}/Saves.meta";
    public static readonly string SettingsSavePath = $"{SavePath}/Settings.json";

    public bool HasSaveData => File.Exists(SaveFilePath);

    private JObject _saveData;
    private readonly BinaryFormatter _binaryFormatter = new();

    public void Init()
    {
        var directory = new DirectoryInfo(SavePath);
        if (!directory.Exists)
        {
            directory.Create();
        }

        if (LoadFromFile(SaveFilePath, out var json))
        {
            _saveData = JObject.Parse(json);
        }
    }

    public void Save()
    {
        if (Managers.Scene.CurrentScene is GameScene)
        {
            var saveData = new JObject()
            {
                { SceneManagerEx.SaveKey, Managers.Scene.GetSaveData() },
            };

            SaveToFile(SaveFilePath, saveData.ToString());
        }
    }

    public bool Load<T>(string saveKey, out T saveData) where T : class
    {
        saveData = null;

        if (_saveData != null)
        {
            var token = _saveData.GetValue(saveKey);
            if (token != null)
            {
                saveData = token.ToObject<T>();
            }
        }

        return saveData != null;
    }

    public void ClearSaveData()
    {
        File.Delete(SaveFilePath);
        File.Delete(SaveMetaFilePath);
        _saveData?.RemoveAll();
    }

    private void SaveToFile(string path, string json)
    {
        using var stream = new FileStream(path, FileMode.Create);
        _binaryFormatter.Serialize(stream, json);
    }

    private bool LoadFromFile(string path, out string json)
    {
        json = null;

        if (File.Exists(path))
        {
            using var stream = new FileStream(path, FileMode.Open);
            json = _binaryFormatter.Deserialize(stream) as string;
            return true;
        }
        else
        {
            Debug.Log($"[DataManager] No have save data : {path}");
        }

        return false;
    }
}
