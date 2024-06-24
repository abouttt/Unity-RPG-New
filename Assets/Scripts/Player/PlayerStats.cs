using System;
using UnityEngine;

[Serializable]
public class PlayerStats
{
    [field: SerializeField]
    public BasicStats BasicStats = new();

    public int XP;

    public int HP
    {
        get => BasicStats.HP;
        set => BasicStats.HP = value;
    }

    public int MP
    {
        get => BasicStats.MP;
        set => BasicStats.MP = value;
    }

    public float SP
    {
        get => BasicStats.SP;
        set => BasicStats.SP = value;
    }

    public int Damage
    {
        get => BasicStats.Damage;
        set => BasicStats.Damage = value;
    }

    public int Defense
    {
        get => BasicStats.Defense;
        set => BasicStats.Defense = value;
    }

    public void CalcPercentageStats(BasicStats percentageStats)
    {
        HP = Util.CalcIncreasePercentage(HP, percentageStats.HP);
        MP = Util.CalcIncreasePercentage(MP, percentageStats.MP);
        SP = Util.CalcIncreasePercentage((int)SP, (int)percentageStats.SP);
        Damage = Util.CalcIncreasePercentage(Damage, percentageStats.Damage);
        Defense = Util.CalcIncreasePercentage(Defense, percentageStats.Defense);
    }
}
