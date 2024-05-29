using System;
using UnityEngine;

[Serializable]
public class BasicStats
{
    public int HP;
    public int MP;
    public float SP;
    public int Damage;
    public int Defense;

    public static BasicStats operator +(BasicStats a, BasicStats b)
    {
        return new BasicStats
        {
            HP = a.HP + b.HP,
            MP = a.MP + b.MP,
            SP = a.SP + b.SP,
            Damage = a.Damage + b.Damage,
            Defense = a.Defense + b.Defense,
        };
    }

    public static BasicStats operator -(BasicStats a, BasicStats b)
    {
        return new BasicStats
        {
            HP = a.HP - b.HP,
            MP = a.MP - b.MP,
            SP = a.SP - b.SP,
            Damage = a.Damage - b.Damage,
            Defense = a.Defense - b.Defense,
        };
    }
}
