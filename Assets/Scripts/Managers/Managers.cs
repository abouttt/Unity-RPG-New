using UnityEngine;

public class Managers : Singleton<Managers>
{
    public static CooldownManager Cooldown => Instance._cooldown;
    public static DataManager Data => Instance._data;
    public static GameManager Game => Instance._game;
    public static InputManager Input => Instance._input;
    public static PoolManager Pool => Instance._pool;
    public static QuestManager Quest => Instance._quest;
    public static ResourceManager Resource => Instance._resource;
    public static SceneManagerEx Scene => Instance._scene;
    public static SoundManager Sound => Instance._sound;
    public static UIManager UI => Instance._ui;

    private static bool s_init = false;

    private readonly CooldownManager _cooldown = new();
    private readonly DataManager _data = new();
    private readonly GameManager _game = new();
    private readonly InputManager _input = new();
    private readonly PoolManager _pool = new();
    private readonly QuestManager _quest = new();
    private readonly ResourceManager _resource = new();
    private readonly SceneManagerEx _scene = new();
    private readonly SoundManager _sound = new();
    private readonly UIManager _ui = new();

    public static void Init()
    {
        if (s_init)
        {
            return;
        }

        Data.Init();
        Input.Init();
        Pool.Init();
        Sound.Init();
        UI.Init();

        s_init = true;
    }

    public static void Clear()
    {
        if (!s_init)
        {
            return;
        }

        if (Instance == null)
        {
            return;
        }

        Cooldown.Clear();
        Input.Clear();
        Pool.Clear();
        Quest.Clear();
        Sound.Clear();
        UI.Clear();

        s_init = false;
    }

    private void LateUpdate()
    {
        _cooldown.Cooling();
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();

        Clear();
    }
}
