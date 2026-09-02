using UnityEngine;

[CreateAssetMenu(fileName = "TowerStats", menuName = "Tower Defense/Tower Stats")]
public class TowerStatsData : ScriptableObject {
    public float daño = 25f;
    public float defensa;
    public float penetracion;
    public float rango = 6f;
    public float tiempoEntreAtaques = 1f;
    public float buffArea;
}
