using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Stats Table", menuName = "Player/Stats Table")]
public class PlayerStatsTable : ScriptableObject
{
    [field: SerializeField]
    public List<PlayerStats> StatsTable { get; private set; }
}
