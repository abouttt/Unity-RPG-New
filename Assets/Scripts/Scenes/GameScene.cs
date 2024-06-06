using UnityEngine;

public class GameScene : BaseScene
{
    [field: SerializeField, Space(10)]
    public string SceneID { get; private set; }

    [field: SerializeField, Space(10)]
    public Vector3 DefaultSpawnPosition { get; private set; }

    [field: SerializeField]
    public float DefaultSpawnYaw { get; private set; }

    protected override void Init()
    {
        base.Init();
        Managers.Data.LoadSettings();

        if (!Managers.Data.HasSaveData)
        {
            Managers.Game.IsDefaultSpawn = true;
        }

        Player.Init();
    }

    private void Start()
    {
        InstantiatePackage("GameUIPackage.prefab");

        Managers.Game.IsDefaultSpawn = false;
        Managers.Game.IsPortalSpawn = false;
        Managers.Input.CursorLocked = true;
        Managers.Quest.ReceiveReport(Category.Scene, SceneID, 1);

        Player.Status.Gold += 10000;
        Player.Status.SkillPoint += 3;

        Managers.UI.Get<UI_TopCanvas>().FadeInitBG();
    }
}
