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
        var package = Managers.Resource.Instantiate("GameUIPackage.prefab");
        package.transform.DetachChildren();
        Destroy(package);
    }

    private void Start()
    {
        Managers.Input.CursorLocked = true;
        Managers.UI.Get<UI_TopCanvas>().FadeInitBG();
    }
}
