using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Database/Item Database", fileName = "ItemDatabase")]
public class ItemDatabase : SingletonScriptableObject<ItemDatabase>
{
    public IReadOnlyCollection<ItemData> Items => _items;

    [SerializeField]
    private List<ItemData> _items;

    public ItemData FindItemById(string id)
    {
        return _items.FirstOrDefault(item => item.ItemId.Equals(id));
    }

#if UNITY_EDITOR
    [ContextMenu("Find Items")]
    public void FindItems()
    {
        FindItemsBy<ItemData>();
    }

    private void FindItemsBy<T>() where T : ItemData
    {
        _items = new();
        foreach (var guid in AssetDatabase.FindAssets($"t:{typeof(T)}"))
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            T item = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            _items.Add(item);
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
