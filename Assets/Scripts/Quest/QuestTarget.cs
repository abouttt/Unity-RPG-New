using System;
using UnityEngine;

[Serializable]
public class QuestTarget
{
    [field: SerializeField]
    public Category Category { get; private set; }

    [field: SerializeField]
    public string TargetId { get; private set; }

    [field: SerializeField]
    public string Description { get; private set; }

    [field: SerializeField]
    public int CompleteCount { get; private set; }

    [field: SerializeField]
    public bool RemoveAfterCompletion { get; private set; }
}
