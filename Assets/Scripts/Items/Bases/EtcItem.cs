using UnityEngine;

public class EtcItem : StackableItem
{
    public EtcItemData EtcData { get; private set; }
    public override IStackableItemData StackableData => EtcData;

    public EtcItem(EtcItemData data, int count = 1)
        : base(data)
    {
        EtcData = data;
        SetCount(count);
    }
}
