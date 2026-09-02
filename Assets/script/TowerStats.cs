using UnityEngine;

public class TowerStats : TowerBaseStats {
    [Header("Datos base")]
    public TowerStatsData datos;
    public TowerType tipo = TowerType.Basica;

    [Header("Estadisticas actuales")]
    private float temporizadorAtaque;

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
    }

    void Update() {
        temporizadorAtaque -= Time.deltaTime;
        if (temporizadorAtaque > 0f) return;

        EnemyStats objetivo = EncontrarObjetivo();
        if (objetivo == null) return;

        objetivo.RecibirDanio(new DamageData(daño));
        temporizadorAtaque = tiempoEntreAtaques;
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
    }

    protected virtual void AplicarPerfilPorTipo() {
        switch (tipo) {
            case TowerType.Francotirador:
                daño = 80f;
                rango = 12f;
                tiempoEntreAtaques = 2.5f;
                break;
            case TowerType.Ametralladora:
                daño = 10f;
                rango = 6f;
                tiempoEntreAtaques = 0.25f;
                break;
            case TowerType.Canon:
                daño = 45f;
                rango = 7f;
                tiempoEntreAtaques = 2f;
                break;
            default:
                daño = 25f;
                rango = 6f;
                tiempoEntreAtaques = 1f;
                break;
        }
    }

    EnemyStats EncontrarObjetivo() {
        Collider[] colisiones = Physics.OverlapSphere(transform.position, rango);
        EnemyStats mejorObjetivo = null;
        float distanciaMasCorta = float.MaxValue;

        foreach (Collider colision in colisiones) {
            EnemyStats enemigo = colision.GetComponentInParent<EnemyStats>();
            if (enemigo == null || enemigo.vidaActual <= 0f) continue;

            float distancia = (enemigo.transform.position - transform.position).sqrMagnitude;
            if (distancia < distanciaMasCorta) {
                distanciaMasCorta = distancia;
                mejorObjetivo = enemigo;
            }
        }

        return mejorObjetivo;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rango);
    }
}
