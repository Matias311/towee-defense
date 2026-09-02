using UnityEngine;

public class EnemyStats : EnemyBaseStats {
    [Header("Datos base")]
    public EnemyStatsData datos;

    [Header("Estadisticas actuales")]
    public float vidaMaxima = 100f;
    public float vidaActual = 100f;
    public float daño = 10f;
    public float defensa;
    public float penetracion;
    public DamageType tipoDaño = DamageType.Fisico;
    public AlteredDamageType tipoDañoAlterado = AlteredDamageType.Veneno;
    public float dañoAlterado;
    public EnemyPassiveType pasiva = EnemyPassiveType.Ninguna;
    public float bonusPasiva;
    public int recompensa = 10;

    private bool configurado;
    private float[] dañoAlteradoAcumulado = new float[5];
    private float[] tiempoAlteradoRestante = new float[5];

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

        EnemyStatsData datosAplicados = datos;
        float multiplicadorNivel = 1f + 0.25f * Mathf.Max(0, nivel - 1);
        float multiplicadorWave = 1f + 0.1f * Mathf.Max(0, wave - 1);
        float multiplicador = multiplicadorNivel * multiplicadorWave;

        float vidaBase = datosAplicados != null ? datosAplicados.vida : ObtenerVidaPorTipo(tipo);
        float dañoBase = datosAplicados != null ? datosAplicados.daño : ObtenerDañoPorTipo(tipo);
        float defensaBase = datosAplicados != null ? datosAplicados.defensa : ObtenerDefensaPorTipo(tipo);

        vidaMaxima = vidaBase * multiplicador;
        vidaActual = vidaMaxima;
        daño = dañoBase * multiplicador;
        defensa = defensaBase * multiplicador;
        penetracion = datosAplicados != null
            ? datosAplicados.penetracion * multiplicador
            : 0f;
        tipoDaño = datosAplicados != null ? datosAplicados.tipoDaño : DamageType.Fisico;
        tipoDañoAlterado = datosAplicados != null
            ? datosAplicados.tipoDañoAlterado
            : AlteredDamageType.Veneno;
        dañoAlterado = datosAplicados != null
            ? datosAplicados.dañoAlterado * multiplicador
            : 0f;
        pasiva = datosAplicados != null
            ? datosAplicados.pasiva
            : EnemyPassiveType.Ninguna;
        bonusPasiva = datosAplicados != null ? datosAplicados.bonusPasiva : 0f;
        AplicarPerfilAutomatico(tipo, datosAplicados == null);
        recompensa = datosAplicados != null
            ? Mathf.RoundToInt(datosAplicados.recompensa * multiplicador)
            : Mathf.RoundToInt(10f * multiplicador);
        AplicarPasiva();
        configurado = true;
    }

    public DamageData CrearDanio() {
        return new DamageData(
            tipoDaño == DamageType.Alterado ? dañoAlterado : daño,
            tipoDaño,
            penetracion,
            tipoDañoAlterado,
            datos != null ? datos.duracionDañoAlterado : 3f
        );
    }

    public void RecibirDanio(float cantidad) {
        RecibirDanio(new DamageData(cantidad));
    }

    public override void RecibirDanio(DamageData dañoRecibido) {
        if (dañoRecibido.cantidad <= 0f || vidaActual <= 0f) return;

        if (dañoRecibido.tipo == DamageType.Alterado) {
            int indice = (int)dañoRecibido.tipoAlterado;
            dañoAlteradoAcumulado[indice] += dañoRecibido.cantidad;
            tiempoAlteradoRestante[indice] = Mathf.Max(
                tiempoAlteradoRestante[indice],
                dañoRecibido.duracion
            );
            return;
        }

        float dañoFinal = dañoRecibido.tipo == DamageType.Verdadero
            ? dañoRecibido.cantidad
            : dañoRecibido.cantidad * (
                100f / (100f + Mathf.Max(0f, defensa - dañoRecibido.penetracion))
            );

        vidaActual = Mathf.Max(0f, vidaActual - dañoFinal);
        if (vidaActual <= 0f) {
            Morir();
        }
    }

    void Update() {
        for (int i = 0; i < dañoAlteradoAcumulado.Length; i++) {
            if (tiempoAlteradoRestante[i] <= 0f) continue;

            tiempoAlteradoRestante[i] -= Time.deltaTime;
            RecibirDanio(new DamageData(
                dañoAlteradoAcumulado[i] * Time.deltaTime,
                DamageType.Verdadero
            ));
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
        defensa = datos != null ? datos.defensa : defensa;
        penetracion = datos != null ? datos.penetracion : penetracion;
        tipoDaño = datos != null ? datos.tipoDaño : tipoDaño;
        tipoDañoAlterado = datos != null ? datos.tipoDañoAlterado : tipoDañoAlterado;
        dañoAlterado = datos != null ? datos.dañoAlterado : dañoAlterado;
        pasiva = datos != null ? datos.pasiva : pasiva;
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

    float ObtenerDefensaPorTipo(EnemyType tipo) {
        switch (tipo) {
            case EnemyType.Special: return 15f;
            case EnemyType.UltraPro: return 30f;
            case EnemyType.BossL1: return 50f;
            case EnemyType.BossL2: return 75f;
            case EnemyType.BossL3: return 100f;
            case EnemyType.BossL4: return 140f;
            default: return 5f;
        }
    }

    void AplicarPerfilAutomatico(EnemyType tipo, bool usarPerfil) {
        if (!usarPerfil) return;

        switch (tipo) {
            case EnemyType.Special:
                pasiva = EnemyPassiveType.Resistente;
                bonusPasiva = 0.15f;
                break;
            case EnemyType.UltraPro:
                pasiva = EnemyPassiveType.Frenetico;
                bonusPasiva = 0.2f;
                break;
            case EnemyType.BossL1:
                pasiva = EnemyPassiveType.Blindado;
                bonusPasiva = 0.1f;
                break;
            case EnemyType.BossL2:
                pasiva = EnemyPassiveType.Blindado;
                bonusPasiva = 0.2f;
                break;
            case EnemyType.BossL3:
                pasiva = EnemyPassiveType.Regenerativo;
                bonusPasiva = 0.25f;
                break;
            case EnemyType.BossL4:
                pasiva = EnemyPassiveType.Blindado;
                bonusPasiva = 0.3f;
                break;
            default:
                pasiva = EnemyPassiveType.Ninguna;
                bonusPasiva = 0f;
                break;
        }

        if (tipo == EnemyType.Special || tipo == EnemyType.UltraPro) {
            tipoDaño = DamageType.Alterado;
            tipoDañoAlterado = tipo == EnemyType.Special
                ? AlteredDamageType.Veneno
                : AlteredDamageType.Fuego;
            dañoAlterado = daño * 0.25f;
        }
    }

    void AplicarPasiva() {
        float bonus = Mathf.Max(0f, datos != null ? datos.bonusPasiva : bonusPasiva);
        switch (pasiva) {
            case EnemyPassiveType.Resistente:
                defensa *= 1f + bonus;
                break;
            case EnemyPassiveType.Frenetico:
                daño *= 1f + bonus;
                break;
            case EnemyPassiveType.Blindado:
                defensa *= 1f + bonus * 2f;
                break;
            case EnemyPassiveType.Regenerativo:
                vidaMaxima *= 1f + bonus;
                vidaActual = vidaMaxima;
                break;
        }
    }
}
