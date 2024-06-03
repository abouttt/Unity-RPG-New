using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumable/HealPotion", fileName = "Item_Consumable_HealPotion")]
public class HealPotionData : ConsumableItemData
{
    [SerializeField]
    private int _healAmount;

    public override void Use<T>(T inventory, Item item)
    {
        Player.Status.HP += _healAmount;
    }
}
