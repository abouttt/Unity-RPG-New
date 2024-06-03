using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestTrackerFixed : UI_Base
{
    enum Transforms
    {
        QuestTracker,
    }

    [SerializeField]
    private int _limitCount;

    private readonly Dictionary<Quest, UI_QuestTrackerSubitem> _subitems = new();

    protected override void Init()
    {
        Managers.UI.Register<UI_QuestTrackerFixed>(this);

        BindRT(typeof(Transforms));
    }

    public bool AddTracker(Quest quest)
    {
        if (_subitems.Count == _limitCount)
        {
            return false;
        }

        if (_subitems.TryGetValue(quest, out var _))
        {
            return false;
        }

        var go = Managers.Resource.Instantiate("UI_QuestTrackerSubitem.prefab", GetRT((int)Transforms.QuestTracker), true);
        var subitem = go.GetComponent<UI_QuestTrackerSubitem>();
        subitem.SetQuest(quest);
        _subitems.Add(quest, subitem);
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetRT((int)Transforms.QuestTracker));

        return true;
    }

    public void RemoveTracker(Quest quest)
    {
        if (_subitems.TryGetValue(quest, out var tracker))
        {
            tracker.Clear();
            _subitems.Remove(quest);
            Managers.Resource.Destroy(tracker.gameObject);
        }
    }
}
