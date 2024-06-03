using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    public string IconName { get; private set; }

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Minimap");
    }

    public void Setup(string spriteName, string iconName, float scale)
    {
        IconName = iconName;
        transform.localScale = new Vector3(scale, scale, scale);
        GetComponent<SpriteRenderer>().sprite = Managers.Resource.Load<Texture2D>(spriteName).ToSprite();
        GetComponent<SphereCollider>().radius = scale;
    }
}
