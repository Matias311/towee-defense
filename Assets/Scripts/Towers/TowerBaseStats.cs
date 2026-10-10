using UnityEngine;

public abstract class TowerBaseStats : MonoBehaviour {
    [Header("Estadisticas actuales")]
    public float daño = 25f;
    public float defensa;
    public float penetracion;
    public float rango = 6f;
    public float tiempoEntreAtaques = 1f;
    public float buffArea;

    public abstract void Configurar(TowerStatsData datosConfigurados);
}
