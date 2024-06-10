using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public bool CanAttack { get; set; }
    public bool CanParry { get; set; }
    public bool CanDefense { get; set; }
    public bool CanSkill { get; set; }

    public bool IsAttacking { get; private set; }
    public bool IsParrying { get; private set; }
    public bool IsDefending { get; private set; }
    public bool IsDamaging { get; private set; }
    public bool IsDefenseDamaging { get; private set; }

    public Weapon Weapon { get; private set; }

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            CanAttack = value;
            CanParry = value;
            CanDefense = value;
            CanSkill = value;
        }
    }

    [SerializeField]
    private float _defenseAngle;

    [SerializeField]
    private float _attackRequiredSP;

    [SerializeField]
    private float _parryRequiredSP;

    [SerializeField]
    private float _defenseDamagedRequiredSP;

    private bool _hasReservedAttack;
    private bool _isParryable;
    private bool _hasShield;
    private bool _enabled;

    // animation IDs
    private readonly int _animIDAttack = Animator.StringToHash("Attack");
    private readonly int _animIDParry = Animator.StringToHash("Parry");
    private readonly int _animIDDefense = Animator.StringToHash("Defense");
    private readonly int _animIDDamaged = Animator.StringToHash("Damaged");
    private readonly int _animIDSkill = Animator.StringToHash("Skill");

    private void Awake()
    {
        Player.EquipmentInventory.InventoryChanged += Refresh;
    }

    private void Start()
    {
        Enabled = true;
        Refresh(EquipmentType.Weapon);
        Refresh(EquipmentType.Shield);
    }

    private void Update()
    {
        if (!Managers.Input.CursorLocked)
        {
            _hasReservedAttack = false;
            if (IsDefending)
            {
                OffDefense();
            }
            return;
        }

        if (_hasReservedAttack)
        {
            Attack();
            return;
        }

        if (Managers.Input.Defense && CanDefense && _hasShield)
        {
            Defense();
        }
        else if (IsDefending)
        {
            OffDefense();
        }
    }

    public void Clear()
    {
        IsAttacking = false;
        IsParrying = false;
        IsDefending = false;
        IsDefenseDamaging = false;
        IsDamaging = false;
        _hasReservedAttack = false;
        _isParryable = false;
        Player.Animator.SetBool(_animIDDefense, false);
    }

    private void Attack()
    {
        if (!CanAttack)
        {
            return;
        }

        _hasReservedAttack = false;

        if (Player.Status.SP <= 0f)
        {
            return;
        }

        if (IsDefending)
        {
            OffDefense();
            CanDefense = false;
        }

        IsAttacking = true;
        Player.Movement.CanRotation = true;
        Player.Animator.SetBool(_animIDAttack, true);
    }

    private void Parry(InputAction.CallbackContext context)
    {
        if (!CanParry)
        {
            return;
        }

        if (Player.Status.SP <= 0f)
        {
            return;
        }

        if (IsDefending)
        {
            OffDefense();
            CanDefense = false;
        }

        IsParrying = true;
        Player.Movement.CanRotation = true;
        Player.Animator.SetBool(_animIDParry, true);
    }

    private void Defense()
    {
        IsDefending = true;
        Player.Animator.SetBool(_animIDDefense, true);
    }

    private void OffDefense()
    {
        IsDefending = false;
        Player.Animator.SetBool(_animIDDefense, false);
    }

    private void ReserveAttack(InputAction.CallbackContext context)
    {
        _hasReservedAttack = true;
    }

    private void Refresh(EquipmentType equipmentType)
    {
        switch (equipmentType)
        {
            case EquipmentType.Weapon:
                if (Player.EquipmentInventory.IsEquipped(equipmentType))
                {
                    Managers.Input.GetAction("Attack").performed += ReserveAttack;
                    Weapon = Player.Root.GetEquipment(EquipmentType.Weapon).GetComponent<Weapon>();
                }
                else
                {
                    Managers.Input.GetAction("Attack").performed -= ReserveAttack;
                    Weapon = null;
                }
                break;
            case EquipmentType.Shield:
                _hasShield = Player.EquipmentInventory.IsEquipped(equipmentType);
                if (_hasShield)
                {
                    Managers.Input.GetAction("Parry").performed += Parry;
                }
                else
                {
                    Managers.Input.GetAction("Parry").performed -= Parry;
                }
                break;
            default:
                break;
        }
    }

    private void OnBeginAttack()
    {
        Player.Status.SP -= _attackRequiredSP;
        Player.Animator.SetBool(_animIDAttack, false);
    }

    private void OnCanAttackCombo()
    {
        CanAttack = true;
        OnCanParryAndRoll();
    }

    private void OnEnableWeapon()
    {
        Weapon.Enabled = true;
    }

    private void OnDisableWeapon()
    {
        Weapon.Enabled = false;
    }

    private void OnBeginParry()
    {
        Player.Status.SP -= _parryRequiredSP;
        Player.Animator.SetBool(_animIDParry, false);
    }

    private void OnEnableParry()
    {
        _isParryable = true;
    }

    private void OnDisableParry()
    {
        _isParryable = false;
    }

    private void OnCanParryAndRoll()
    {
        CanParry = true;
        CanSkill = true;
        Player.Movement.CanRoll = true;
    }

    private void OnBeginDefenseDamaged()
    {
        Player.Status.SP -= _defenseDamagedRequiredSP;
    }

    private void OnBeginDamaged()
    {
        Player.Animator.SetBool(_animIDDamaged, false);
    }
}
