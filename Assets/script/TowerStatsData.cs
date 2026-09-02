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
}
