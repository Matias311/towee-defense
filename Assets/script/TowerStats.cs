using UnityEngine;

public class TowerStats : TowerBaseStats {
    [Header("Datos base")]
    public TowerStatsData datos;
    public TowerType tipo = TowerType.Basica;
    public TowerAttackType tipoAtaque = TowerAttackType.Directo;
    public TowerTargetPriority prioridadObjetivo = TowerTargetPriority.MasCercano;
    public float radioArea = 2.5f;
    public int cantidadObjetivos = 1;

    void Awake() {
        AplicarDatosBase();
    }

    public override void Configurar(TowerStatsData datosConfigurados) {
        if (datosConfigurados == null) {
            AplicarPerfilPorTipo();
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
