using UnityEngine;

public class EnemyStats : MonoBehaviour {
    [Header("Datos base")]
    public EnemyStatsData datos;

    [Header("Estadisticas actuales")]
    public float vidaMaxima = 100f;
    public float vidaActual = 100f;
    public float daño = 10f;
    public int recompensa = 10;

    private bool configurado;

    void Awake() {
        AplicarDatosBase();
    }

    public void Configurar(
        EnemyType tipo,
        int nivel,
        int wave,
        EnemyStatsData datosConfigurados = null
    ) {
        if (datosConfigurados != null) {
            datos = datosConfigurados;
        }

        float multiplicadorNivel = 1f + 0.25f * Mathf.Max(0, nivel - 1);
        float multiplicadorWave = 1f + 0.1f * Mathf.Max(0, wave - 1);
        float multiplicador = multiplicadorNivel * multiplicadorWave;

        float vidaBase = datos != null ? datos.vida : ObtenerVidaPorTipo(tipo);
        float dañoBase = datos != null ? datos.daño : ObtenerDañoPorTipo(tipo);

        vidaMaxima = vidaBase * multiplicador;
        vidaActual = vidaMaxima;
        daño = dañoBase * multiplicador;
        recompensa = datos != null ? datos.recompensa : Mathf.RoundToInt(10f * multiplicador);
        configurado = true;
    }

    public void RecibirDanio(float cantidad) {
        if (cantidad <= 0f || vidaActual <= 0f) return;

        vidaActual = Mathf.Max(0f, vidaActual - cantidad);
        if (vidaActual <= 0f) {
            Morir();
        }
    }

    void Morir() {
        TowerPlacer towerPlacer = FindAnyObjectByType<TowerPlacer>();
        if (towerPlacer != null) {
            towerPlacer.RegistrarPuntos(recompensa);
        }

        Destroy(gameObject);
    }

    void AplicarDatosBase() {
        if (configurado) return;

        vidaMaxima = datos != null ? datos.vida : vidaMaxima;
        vidaActual = vidaMaxima;
        daño = datos != null ? datos.daño : daño;
        recompensa = datos != null ? datos.recompensa : recompensa;
    }

    float ObtenerVidaPorTipo(EnemyType tipo) {
        switch (tipo) {
            case EnemyType.Special: return 150f;
            case EnemyType.UltraPro: return 225f;
            case EnemyType.BossL1: return 500f;
            case EnemyType.BossL2: return 700f;
            case EnemyType.BossL3: return 900f;
            case EnemyType.BossL4: return 1200f;
            default: return 100f;
        }
    }

    float ObtenerDañoPorTipo(EnemyType tipo) {
        switch (tipo) {
            case EnemyType.Special: return 15f;
            case EnemyType.UltraPro: return 25f;
            case EnemyType.BossL1: return 35f;
            case EnemyType.BossL2: return 45f;
            case EnemyType.BossL3: return 60f;
            case EnemyType.BossL4: return 80f;
            default: return 10f;
        }
    }
}
