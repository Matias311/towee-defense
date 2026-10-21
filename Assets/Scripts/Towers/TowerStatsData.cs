using UnityEngine;

[System.Serializable]
public enum TowerType {
    Basica,
    Francotirador,
    Ametralladora,
    Canon
}

public enum TowerAttackType {
    Directo,
    Area
}

public enum TowerTargetPriority {
    MasCercano,
    MasLejano,
    MasVida
}

[CreateAssetMenu(fileName = "TowerStats", menuName = "Tower Defense/Tower Stats")]
public class TowerStatsData : ScriptableObject {
    public TowerType tipo = TowerType.Basica;
    public float daño = 25f;
    public float defensa;
    public float penetracion;
    public float rango = 6f;
    public float tiempoEntreAtaques = 1f;
    public float buffArea;
    public TowerAttackType tipoAtaque = TowerAttackType.Directo;
    public TowerTargetPriority prioridadObjetivo = TowerTargetPriority.MasCercano;
    public float radioArea = 2.5f;
    public int cantidadObjetivos = 1;
    public bool usarProyectil = true;
    public GameObject prefabProyectil;
    public float velocidadProyectil = 14f;
    [Header("Efectos al impactar")]
    [Range(0f, 95f), Tooltip("Porcentaje de velocidad que pierde el enemigo al recibir el impacto.")]
    public float ralentizacionImpacto;
    [Min(0f), Tooltip("Duración en segundos de la ralentización.")]
    public float duracionRalentizacion;
}
