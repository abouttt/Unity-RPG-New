using System;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(menuName = "Settings/Scene Settings", fileName = "SceneSettings")]
public class SceneSettings : SingletonScriptableObject<SceneSettings>
{
    [Serializable]
    public class Settings
    {
        [field: SerializeField]
        public AddressableLabel[] ResourcesLoadLabels { get; private set; }

        [field: SerializeField]
        public bool ReloadScene { get; private set; }

        [field: SerializeField]
        public bool ClearResourcesWhenEndScene { get; private set; }

        [field: SerializeField]
        public Sprite BackgroundImage { get; private set; }

        [field: SerializeField]
        public AudioClip SceneBGM { get; private set; }
    }

    public Settings this[SceneType sceneType]
    {
        get
        {
            return _settings[sceneType];
        }
    }

    [SerializeField]
    private SerializedDictionary<SceneType, Settings> _settings;
}
