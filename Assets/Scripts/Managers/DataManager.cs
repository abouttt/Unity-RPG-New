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
                { PlayerMovement.SaveKey, Player.Movement.GetSaveData() },
                { ItemInventory.SaveKey, Player.ItemInventory.GetSaveData() },
            };

            SaveToFile(SaveFilePath, saveData.ToString());
        }

        SaveSettings();
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

    public void SaveSettings()
    {
        var settingsSaveData = new SettingsSaveData()
        {
            BGMVolume = Managers.Sound.GetAudioSource(SoundType.BGM).volume,
            EffectVolume = Managers.Sound.GetAudioSource(SoundType.Effect).volume,
            MSAA = QualitySettings.antiAliasing,
            Frame = Application.targetFrameRate,
            VSync = QualitySettings.vSyncCount
        };

        SaveToFile(SettingsSavePath, JsonUtility.ToJson(settingsSaveData));
    }

    public void LoadSettings()
    {
        if (!LoadFromFile(SettingsSavePath, out var json))
        {
            return;
        }

        var settingsSaveData = JsonUtility.FromJson<SettingsSaveData>(json);
        Managers.Sound.GetAudioSource(SoundType.BGM).volume = settingsSaveData.BGMVolume;
        Managers.Sound.GetAudioSource(SoundType.Effect).volume = settingsSaveData.BGMVolume;
        QualitySettings.antiAliasing = settingsSaveData.MSAA;
        Application.targetFrameRate = settingsSaveData.Frame;
        QualitySettings.vSyncCount = settingsSaveData.VSync;
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
