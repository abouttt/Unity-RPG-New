using UnityEngine;

public class Player : MonoBehaviour
{
    public static GameObject GameObject { get; private set; }
    public static Transform Transform { get; private set; }
    public static Collider Collider { get; private set; }
    public static Animator Animator { get; private set; }
    public static PlayerStatus Status { get; private set; }
    public static PlayerMovement Movement { get; private set; }
    public static PlayerCamera Camera { get; private set; }
    public static ItemInventory ItemInventory { get; private set; }

    private void Awake()
    {
        GameObject = gameObject;
        Transform = transform;
        Collider = GetComponent<Collider>();
        Animator = GetComponent<Animator>();
        Status = GetComponent<PlayerStatus>();
        Movement = GetComponent<PlayerMovement>();
        Camera = GetComponent<PlayerCamera>();
        ItemInventory = GetComponent<ItemInventory>();
    }

    public void Init()
    {
        GameObject = gameObject;
        Transform = transform;
        Collider = GetComponent<Collider>();
        Animator = GetComponent<Animator>();
        Status = GetComponent<PlayerStatus>();
        Movement = GetComponent<PlayerMovement>();
        Camera = GetComponent<PlayerCamera>();
        ItemInventory = GetComponent<ItemInventory>();
    }
}
