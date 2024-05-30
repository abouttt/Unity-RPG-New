using UnityEngine;

public class ChestItem : FieldItem
{
    [SerializeField]
    private Animation _chestAnimation;

    [SerializeField]
    private AnimationClip _openAnimation;

    [SerializeField]
    private AnimationClip _closeAnimation;

    [SerializeField]
    private ParticleSystem _hasItemParticle;

    [SerializeField]
    private ParticleSystem _openItemParticle;

    public override void Interaction()
    {
        base.Interaction();

        _openItemParticle.Play();
        _chestAnimation.clip = _openAnimation;
        _chestAnimation.Play();
    }

    public override void Deinteraction()
    {
        base.Deinteraction();

        _chestAnimation.clip = _closeAnimation;
        _chestAnimation.Play();
    }

    public override void RemoveItem(ItemData itemData, int count)
    {
        base.RemoveItem(itemData, count);

        if (Items.Count == 0)
        {
            _hasItemParticle.Stop();
        }
    }
}
