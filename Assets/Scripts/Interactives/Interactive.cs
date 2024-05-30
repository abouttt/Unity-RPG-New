using UnityEngine;

public abstract class Interactive : MonoBehaviour
{
    public bool IsDetected { get; set; }            // 감지 되었는지
    public bool IsInteracted { get; private set; }  // 상호작용 중인지

    [field: SerializeField]
    public string InteractionMessage { get; protected set; }

    [field: SerializeField]
    public Vector3 InteractionKeyGuidePos { get; protected set; }

    [field: SerializeField]
    public float InteractionInputTime { get; protected set; }

    [field: SerializeField]
    public bool CanInteraction { get; protected set; } = true;

    protected virtual void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactive");
    }

    public virtual void Interaction()
    {
        IsInteracted = true;
    }

    public virtual void Deinteraction()
    {
        IsInteracted = false;
    }

    private void OnDrawGizmosSelected()
    {
        // InteractionKeyGuidePos 위치 시각화
        Gizmos.DrawWireSphere(transform.position + InteractionKeyGuidePos, 0.1f);
    }
}
