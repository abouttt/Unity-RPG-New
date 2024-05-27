using UnityEngine;

public class EtcItem : StackableItem
{
    public EtcItemData EtcData { get; private set; }

    public EtcItem(EtcItemData data, int count = 1)
        : base(data, count)
    {
        EtcData = data;
    }
}
