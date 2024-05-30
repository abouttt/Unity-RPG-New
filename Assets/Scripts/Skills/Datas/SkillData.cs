using UnityEngine;
using AYellowpaper.SerializedCollections;

public abstract class SkillData : ScriptableObject
{
    [field: SerializeField]
    public string SkillId { get; private set; }

    [field: SerializeField]
    public string SkillName { get; private set; }

    [field: SerializeField]
    public Sprite SkillImage { get; private set; }

    [field: SerializeField, ReadOnly]
    public SkillType SkillType { get; protected set; }

    [field: SerializeField, TextArea]
    public string Description { get; private set; }

    [field: SerializeField]
    public int MaxLevel { get; private set; }

    [field: SerializeField]
    public int LimitLevel { get; private set; }

    [field: SerializeField]
    public int RequiredSkillPoint { get; private set; }

    [field: SerializeField]
    public BasicStats[] FixedStats { get; private set; }

    [field: SerializeField]
    public BasicStats[] PercentageStats { get; private set; }

    [field: SerializeField]
    public bool Root { get; private set; }

    [field: SerializeField, SerializedDictionary("자식 스킬", "레벨 조건")]
    public SerializedDictionary<SkillData, int> Children { get; private set; }

    public abstract Skill CreateSkill(int level);

    public bool Equals(SkillData other)
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

        return SkillId.Equals(other.SkillId);
    }
}
