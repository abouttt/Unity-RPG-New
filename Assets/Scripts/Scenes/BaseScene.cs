using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    [field: SerializeField]
    public SceneType SceneType { get; private set; } = SceneType.Unknown;

    [SerializeField]
    private AudioClip _sceneBGM;

    [SerializeField]
    private bool _reloadScene;

    private void Awake()
    {
        if (_reloadScene && Managers.Resource.ResourceCount == 0)
        {
            Managers.Scene.LoadScene(Managers.Scene.CurrentScene.SceneType);
        }
        else
        {
            Managers.Init();
            Init();
        }
    }

    protected virtual void Init()
    {
        if (FindObjectOfType(typeof(EventSystem)) == null)
        {
            Managers.Resource.Instantiate("EventSystem.prefab");
        }
    }

    protected void InstantiatePackage(string packageName)
    {
        var package = Managers.Resource.Instantiate(packageName);
        package.transform.DetachChildren();
        Destroy(package);
    }

    protected void OnDestroy()
    {
        Managers.Clear();
    }
}
