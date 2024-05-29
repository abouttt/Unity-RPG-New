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
        FindAnyObjectByType<Player>().Init();
    }

    private void Start()
    {
        var package = Managers.Resource.Instantiate("GameUIPackage.prefab");
        package.transform.DetachChildren();
        Destroy(package);

        Managers.Input.CursorLocked = true;
        Player.Status.Gold += 10000;
        Player.Status.SkillPoint += 3;
        Managers.UI.Get<UI_TopCanvas>().FadeInitBG();
    }
}
