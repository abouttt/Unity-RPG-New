using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumable/HealPotion", fileName = "Item_Consumable_HealPotion")]
public class HealPotionData : ConsumableItemData
{
    [SerializeField]
    private int _healAmount;

    public override bool Use<T>(T inventory, Item item)
    {
        if (!base.Use(inventory, item))
        {
            return false;
        }

        Player.Status.HP += _healAmount;
        Managers.Resource.Instantiate("HealOnceBurst.prefab", Player.Collider.bounds.center, Player.Transform, true);

        return true;
    }
}
