using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Tower Defense/Enemy Stats")]
public class EnemyStatsData : ScriptableObject {
    public float vida = 100f;
    public float daño = 10f;
    public int recompensa = 10;
}
