using UnityEngine;

public class Managers : Singleton<Managers>
{
    public static CooldownManager Cooldown => Instance._cooldown;
    public static InputManager Input => Instance._input;
    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;
    public static SceneManagerEx Scene => Instance._scene;
    public static SoundManager Sound => Instance._sound;
    public static UIManager UI => Instance._ui;

    private static bool s_init = false;

    private readonly CooldownManager _cooldown = new();
    private readonly InputManager _input = new();
    private readonly PoolManager _pool = new();
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
        Sound.Clear();
        UI.Clear();

        s_init = false;
    }

    private void LateUpdate()
    {
        _cooldown.Cooling();
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
