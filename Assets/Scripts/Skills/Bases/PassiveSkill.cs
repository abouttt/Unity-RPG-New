using UnityEngine;

public class PassiveSkill : Skill
{
    public PassiveSkillData PassiveData { get; private set; }

    public PassiveSkill(PassiveSkillData data, int level)
       : base(data, level)
    {
        PassiveData = data;

        if (level > 0)
        {
            if (Data.PercentageStats.Length > 0)
            {
                Player.Status.PercentageStats.Add(Data.PercentageStats[CurrentLevel - 1]);
            }

            if (Data.FixedStats.Length > 0)
            {
                Player.Status.FixedStats.Add(Data.FixedStats[CurrentLevel - 1]);
            }
        }
    }

    public override void LevelUp()
    {
        base.LevelUp();

        if (CurrentLevel > 1)
        {
            if (Data.PercentageStats.Length > 0)
            {
                Player.Status.PercentageStats.Sub(Data.PercentageStats[CurrentLevel - 2]);
                Player.Status.PercentageStats.Add(Data.PercentageStats[CurrentLevel - 1]);
            }

            if (Data.FixedStats.Length > 0)
            {
                Player.Status.FixedStats.Sub(Data.FixedStats[CurrentLevel - 2]);
                Player.Status.FixedStats.Add(Data.FixedStats[CurrentLevel - 1]);
            }
        }
        else
        {
            if (Data.PercentageStats.Length > 0)
            {
                Player.Status.PercentageStats.Add(Data.PercentageStats[CurrentLevel - 1]);
            }

            if (Data.FixedStats.Length > 0)
            {
                Player.Status.FixedStats.Add(Data.FixedStats[CurrentLevel - 1]);
            }
        }
    }

    public override int ResetSkill()
    {
        if (IsUnlocked)
        {
            if (Data.PercentageStats.Length > 0)
            {
                Player.Status.PercentageStats.Sub(Data.PercentageStats[CurrentLevel - 1]);
            }

            if (Data.FixedStats.Length > 0)
            {
                Player.Status.FixedStats.Sub(Data.FixedStats[CurrentLevel - 1]);
            }
        }

        return base.ResetSkill();
    }
}
