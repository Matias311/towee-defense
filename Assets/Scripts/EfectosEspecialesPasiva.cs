using System;
using UnityEngine;

[Serializable]
public class CombatePasiva
{
    [Range(0, 100)] public float probabilidadCritico;
    [Min(1)] public float multiplicadorCritico = 2;
    [Range(0, 100)] public float probabilidadInstaKill;
    public bool instaKillAfectaJefes;
    [Range(0, 100)] public float probabilidadSangrado;
    [Min(0)] public float danioSangrado = 20;
    [Min(.1f)] public float duracionSangrado = 4;
}

[Serializable]
public class MundoPasiva
{
    [Range(0, 100)] public float ralentizacionEnemigos;
    [Range(0, 100)] public float defensaCasa;
    [Min(0)] public float aumentoVelocidadEnemigos;
    [Min(0)] public float aumentoFrecuenciaEnemigos;
    [Range(0, 100), Tooltip("Probabilidad por aparición normal de agregar un UltraPro (o Special si no está configurado).")]
    public float probabilidadExtraRaro;
    [Range(0, 100), Tooltip("Probabilidad por oleada de un enemigo especial o jefe adicional. No entrega pasivas.")]
    public float probabilidadEspecialOJefe;
    [Range(0, 100), Tooltip("Costo único al elegir: porcentaje de vida actual. Ignora defensa.")]
    public float perdidaVidaActual;
    [Min(0), Tooltip("Aumenta permanentemente el daño recibido por la casa (alternativa al costo único).")]
    public float aumentoDanioRecibidoCasa;
}

public struct PerfilImpactoPasiva
{
    public float critico, multiplicadorCritico, instaKill, instaKillJefes;
    public float sangrado, danioSangrado, duracionSangrado;

    public DamageData Resolver(DamageData danio, float vidaObjetivo, bool esJefe, float tiradaInstaKill, float tiradaCritico)
    {
        float probabilidad = esJefe ? instaKillJefes : instaKill;
        if (Ocurre(probabilidad, tiradaInstaKill)) return new DamageData(vidaObjetivo, DamageType.Verdadero);
        if (Ocurre(critico, tiradaCritico)) danio.cantidad *= Mathf.Max(1, multiplicadorCritico);
        return danio;
    }

    public static bool Ocurre(float porcentaje, float tirada) =>
        porcentaje > 0 && (porcentaje >= 100 || Mathf.Clamp01(tirada) < porcentaje / 100f);
}
