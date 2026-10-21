using UnityEngine;

public class TowerStats : TowerBaseStats {
    [Header("Datos base")]
    public TowerStatsData datos;
    public TowerType tipo = TowerType.Basica;
    public TowerAttackType tipoAtaque = TowerAttackType.Directo;
    public TowerTargetPriority prioridadObjetivo = TowerTargetPriority.MasCercano;
    public float radioArea = 2.5f;
    public int cantidadObjetivos = 1;
    public bool usarProyectil = true;
    public GameObject prefabProyectil;
    public float velocidadProyectil = 14f;
    [Range(0f, 95f)] public float ralentizacionImpacto;
    [Min(0f)] public float duracionRalentizacion;

    private float[] baseSinPasivas;
    private float multiplicadorRareza = 1f;
    private float bonoColocacionDanio;
    public float RangoSinAura { get; private set; }
    public void AgregarBonoColocacion(float porcentaje)
    {
        bonoColocacionDanio += Mathf.Max(0, porcentaje);
        RecalcularPasivas();
    }

    protected virtual void Awake() {
        AplicarDatosBase();
        CapturarBase();
    }

    void OnEnable() => RecalcularPasivas();

    private void CapturarBase()
    {
        baseSinPasivas = new[] { daño, rango, 1f / Mathf.Max(.05f, tiempoEntreAtaques), defensa,
            penetracion, radioArea, velocidadProyectil };
        multiplicadorRareza = 1f;
        RecalcularPasivas();
    }

    public void EstablecerRareza(float multiplicador)
    {
        multiplicadorRareza = Mathf.Max(0, multiplicador);
        RecalcularPasivas();
    }

    public void RecalcularPasivas() => RecalcularPasivas(false);

    public void RecalcularPasivas(bool ignorarAura)
    {
        if (baseSinPasivas == null)
        {
            CapturarBase();
            return;
        }
        daño = SistemaPasivas.Calcular(EstadisticaPasiva.Danio, baseSinPasivas[0] * multiplicadorRareza, tipo);
        daño *= 1 + bonoColocacionDanio / 100f;
        RangoSinAura = SistemaPasivas.Calcular(EstadisticaPasiva.Alcance, baseSinPasivas[1], tipo);
        Vector2 aura = ignorarAura ? Vector2.zero : SistemaPasivas.BonoAura(this);
        daño *= 1 + aura.x / 100f;
        rango = RangoSinAura * (1 + aura.y / 100f);
        float ataques = SistemaPasivas.Calcular(EstadisticaPasiva.VelocidadAtaque, baseSinPasivas[2], tipo);
        tiempoEntreAtaques = Mathf.Max(.05f, 1f / Mathf.Max(.001f, ataques));
        defensa = SistemaPasivas.Calcular(EstadisticaPasiva.Defensa, baseSinPasivas[3], tipo);
        penetracion = SistemaPasivas.Calcular(EstadisticaPasiva.Penetracion, baseSinPasivas[4], tipo);
        radioArea = SistemaPasivas.Calcular(EstadisticaPasiva.RadioArea, baseSinPasivas[5], tipo);
        velocidadProyectil = SistemaPasivas.Calcular(EstadisticaPasiva.VelocidadProyectil, baseSinPasivas[6], tipo);
    }

    public override void Configurar(TowerStatsData datosConfigurados) {
        if (baseSinPasivas != null)
        {
            daño = baseSinPasivas[0];
            rango = baseSinPasivas[1];
            tiempoEntreAtaques = 1f / Mathf.Max(.001f, baseSinPasivas[2]);
            defensa = baseSinPasivas[3];
            penetracion = baseSinPasivas[4];
            radioArea = baseSinPasivas[5];
            velocidadProyectil = baseSinPasivas[6];
        }
        if (datosConfigurados == null) {
            AplicarPerfilPorTipo();
            CapturarBase();
            return;
        }

        datos = datosConfigurados;
        tipo = datos.tipo;
        daño = datos.daño;
        defensa = datos.defensa;
        penetracion = datos.penetracion;
        rango = datos.rango;
        tiempoEntreAtaques = Mathf.Max(0.05f, datos.tiempoEntreAtaques);
        buffArea = datos.buffArea;
        tipoAtaque = datos.tipoAtaque;
        prioridadObjetivo = datos.prioridadObjetivo;
        radioArea = Mathf.Max(0f, datos.radioArea);
        cantidadObjetivos = Mathf.Max(1, datos.cantidadObjetivos);
        usarProyectil = datos.usarProyectil;
        prefabProyectil = datos.prefabProyectil;
        velocidadProyectil = Mathf.Max(0.1f, datos.velocidadProyectil);
        ralentizacionImpacto = datos.ralentizacionImpacto;
        duracionRalentizacion = datos.duracionRalentizacion;
        CapturarBase();
        RegistrarLogEstadisticas();
    }

    void AplicarDatosBase() {
        if (datos == null) {
            AplicarPerfilPorTipo();
            return;
        }

        tipo = datos.tipo;
        daño = datos.daño;
        defensa = datos.defensa;
        penetracion = datos.penetracion;
        rango = datos.rango;
        tiempoEntreAtaques = Mathf.Max(0.05f, datos.tiempoEntreAtaques);
        buffArea = datos.buffArea;
        tipoAtaque = datos.tipoAtaque;
        prioridadObjetivo = datos.prioridadObjetivo;
        radioArea = Mathf.Max(0f, datos.radioArea);
        cantidadObjetivos = Mathf.Max(1, datos.cantidadObjetivos);
        usarProyectil = datos.usarProyectil;
        prefabProyectil = datos.prefabProyectil;
        velocidadProyectil = Mathf.Max(0.1f, datos.velocidadProyectil);
        ralentizacionImpacto = datos.ralentizacionImpacto;
        duracionRalentizacion = datos.duracionRalentizacion;
        RegistrarLogEstadisticas();
    }

    protected virtual void AplicarPerfilPorTipo() {
        switch (tipo) {
            case TowerType.Francotirador:
                daño = 80f;
                rango = 12f;
                tiempoEntreAtaques = 2.5f;
                prioridadObjetivo = TowerTargetPriority.MasLejano;
                break;
            case TowerType.Ametralladora:
                daño = 10f;
                rango = 6f;
                tiempoEntreAtaques = 0.25f;
                prioridadObjetivo = TowerTargetPriority.MasCercano;
                break;
            case TowerType.Canon:
                daño = 45f;
                rango = 7f;
                tiempoEntreAtaques = 2f;
                tipoAtaque = TowerAttackType.Area;
                prioridadObjetivo = TowerTargetPriority.MasVida;
                radioArea = 2.5f;
                cantidadObjetivos = 5;
                break;
            default:
                daño = 25f;
                rango = 6f;
                tiempoEntreAtaques = 1f;
                break;
        }
    }

    void RegistrarLogEstadisticas() {
        Debug.Log(
            $"[TOWER] {name} | tipo={tipo}, daño={daño:F1}, " +
            $"tipoAtaque={tipoAtaque}, prioridad={prioridadObjetivo}, " +
            $"defensa={defensa:F1}, penetracion={penetracion:F1}, " +
            $"rango={rango:F1}, cooldown={tiempoEntreAtaques:F2}s, " +
            $"radioArea={radioArea:F1}, objetivos={cantidadObjetivos}"
        );
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rango);
        if (tipoAtaque == TowerAttackType.Area) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radioArea);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, rango);
    }
}
