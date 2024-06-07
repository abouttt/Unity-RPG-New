using System;
using UnityEngine;

[Serializable]
public struct Vector3SaveData
{
    public float X;
    public float Y;
    public float Z;

    public Vector3SaveData(Vector3 vector3)
    {
        X = vector3.x;
        Y = vector3.y;
        Z = vector3.z;
    }

    public Vector3 ToVector3() => new(X, Y, Z);
}

[Serializable]
public struct ItemSaveData
{
    public string ItemId;
    public int Count;
    public int Index;
}

[Serializable]
public struct SettingsSaveData
{
    public float BGMVolume;
    public float EffectVolume;
    public int MSAA;
    public int Frame;
    public int VSync;
}
