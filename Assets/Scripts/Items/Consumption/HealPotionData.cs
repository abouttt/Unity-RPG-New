using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumption/HealPotion", fileName = "Item_Consumption_HealPotion")]
public class HealPotionData : ConsumptionItemData
{
    [SerializeField]
    private int _healAmount;

    public override void Use<T>(T inventory, Item item)
    {
        Player.Status.HP += _healAmount;
    }
}
