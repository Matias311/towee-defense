using UnityEngine;

[System.Serializable]
public enum DamageType {
    Fisico,
    Verdadero,
    Alterado
}

[System.Serializable]
public enum AlteredDamageType {
    Veneno,
    Fuego,
    Hielo,
    Aire,
    Agua
}

[System.Serializable]
public enum EnemyPassiveType {
    Ninguna,
    Resistente,
    Frenetico,
    Blindado,
    Regenerativo
}

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Tower Defense/Enemy Stats")]
public class EnemyStatsData : ScriptableObject {
    public EnemyType tipo = EnemyType.Base;
    public float vida = 100f;
    public float daño = 10f;
    public float defensa;
    public float penetracion;
    public DamageType tipoDaño = DamageType.Fisico;
    public AlteredDamageType tipoDañoAlterado = AlteredDamageType.Veneno;
    public float dañoAlterado;
    public EnemyPassiveType pasiva = EnemyPassiveType.Ninguna;
    [Range(0f, 1f)]
    public float bonusPasiva = 0.1f;
    public float duracionDañoAlterado = 3f;
    public int recompensa = 10;
}
