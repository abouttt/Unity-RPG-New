using UnityEngine;

[CreateAssetMenu(menuName = "Item/Consumable/ManaPotion", fileName = "Item_Consumable_ManaPotion")]
public class ManaPotionData : ConsumableItemData
{
    [SerializeField]
    private int _manaAmount;

    public override bool Use<T>(T inventory, Item item)
    {
        if (!base.Use(inventory, item))
        {
            return false;
        }

        Player.Status.MP += _manaAmount;
        Managers.Resource.Instantiate("ManaOnceBurst.prefab", Player.Collider.bounds.center, Player.Transform, true);

        return true;
    }
}
