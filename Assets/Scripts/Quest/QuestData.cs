using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/QuestData", fileName = "Quest_")]
public class QuestData : ScriptableObject
{
    [field: SerializeField]
    public string QuestId { get; private set; }

    [field: SerializeField]
    public string QuestName { get; private set; }

    [field: SerializeField, TextArea]
    public string Description { get; private set; }

    [field: SerializeField]
    public int LimitLevel { get; private set; }

    [field: SerializeField]
    public string OwnerId { get; private set; }

    [field: SerializeField]
    public string CompleteOwnerId { get; private set; }

    [field: SerializeField]
    public bool CanRemoteComplete { get; private set; }

    [field: SerializeField, Header("Targets")]
    public QuestTarget[] Targets { get; private set; }

    [field: SerializeField]
    public QuestData[] PrerequisiteQuests { get; private set; }

    [Header("Reward")]
    public int RewardGold;
    public int RewardXP;
    public SerializedDictionary<ItemData, int> RewardItems;

    public bool Equals(QuestData other)
    {
        if (other == null)
        {
            return false;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return QuestId.Equals(other.QuestId);
    }
}
