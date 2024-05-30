using UnityEngine;

public abstract class Interactive : MonoBehaviour
{
    public bool IsDetected { get; set; }            // ���� �Ǿ�����
    public bool IsInteracted { get; private set; }  // ��ȣ�ۿ� ������

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
        // InteractionKeyGuidePos ��ġ �ð�ȭ
        Gizmos.DrawWireSphere(transform.position + InteractionKeyGuidePos, 0.1f);
    }
}
