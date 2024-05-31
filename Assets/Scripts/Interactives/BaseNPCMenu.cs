using UnityEngine;

[RequireComponent(typeof(NPC))]
public abstract class BaseNPCMenu : MonoBehaviour
{
    public NPC Owner { get; private set; }
    public string MenuName { get; protected set; }

    protected virtual void Awake()
    {
        Owner = GetComponent<NPC>();
    }

    public abstract void Execution();
}
