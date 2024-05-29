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
}
