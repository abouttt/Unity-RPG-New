using UnityEngine;

public interface IUsableItemData
{
    public int LimitLevel { get; }

    public void Use(int index);
}
