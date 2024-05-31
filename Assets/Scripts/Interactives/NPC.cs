using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactive
{
    [field: SerializeField]
    public string NPCId { get; private set; }

    [field: SerializeField]
    public string NPCName { get; private set; }

    public IReadOnlyCollection<BaseNPCMenu> Menus => _menus;

    private static readonly Dictionary<string, NPC> s_NPCs = new();

    private BaseNPCMenu[] _menus;

    protected override void Awake()
    {
        base.Awake();

        s_NPCs.Add(NPCId, this);
        _menus = GetComponents<BaseNPCMenu>();
    }

    public override void Interaction()
    {
        base.Interaction();
        Managers.UI.Show<UI_NPCMenuPopup>().SetNPC(this);
    }

    private void OnDestroy()
    {
        s_NPCs.Remove(NPCId);
    }
}
