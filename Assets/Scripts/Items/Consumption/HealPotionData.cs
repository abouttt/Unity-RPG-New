using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumption/HealPotion", fileName = "Item_Consumption_HealPotion")]
public class HealPotionData : ConsumptionItemData
{
    [SerializeField]
    private int _healAmount;

    public override void Use()
    {
        Player.Status.HP += _healAmount;
    }
}
