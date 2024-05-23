using System;
using UnityEngine;

[Serializable]
public class PlayerStats
{
    public int HP;
    public int MP;
    public float SP;
    public int XP;
    public int Damage;
    public int Defense;

    public static PlayerStats operator +(PlayerStats a, PlayerStats b)
    {
        return new PlayerStats
        {
            HP = a.HP + b.HP,
            MP = a.MP + b.MP,
            SP = a.SP + b.SP,
            XP = a.XP + b.XP,
            Damage = a.Damage + b.Damage,
            Defense = a.Defense + b.Defense,
        };
    }

    public static PlayerStats operator -(PlayerStats a, PlayerStats b)
    {
        return new PlayerStats
        {
            HP = a.HP - b.HP,
            MP = a.MP - b.MP,
            SP = a.SP - b.SP,
            XP = a.XP - b.XP,
            Damage = a.Damage - b.Damage,
            Defense = a.Defense - b.Defense,
        };
    }
}
