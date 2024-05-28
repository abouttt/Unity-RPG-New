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

    public void Add(BasicStats other)
    {
        HP += other.HP;
        MP += other.MP;
        SP += other.SP;
        Damage += other.Damage;
        Defense += other.Defense;
    }

    public void Sub(BasicStats other)
    {
        HP -= other.HP;
        MP -= other.MP;
        SP -= other.SP;
        Damage -= other.Damage;
        Defense -= other.Defense;
    }
}
