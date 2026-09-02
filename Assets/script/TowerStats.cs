using UnityEngine;

public class TowerStats : TowerBaseStats {
    [Header("Datos base")]
    public TowerStatsData datos;

    [Header("Estadisticas actuales")]
    private float temporizadorAtaque;

    void Awake() {
        AplicarDatosBase();
    }

    public override void Configurar(TowerStatsData datosConfigurados) {
        if (datosConfigurados == null) return;

        datos = datosConfigurados;
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

        objetivo.RecibirDanio(daño);
        temporizadorAtaque = tiempoEntreAtaques;
    }

    void AplicarDatosBase() {
        if (datos == null) return;

        daño = datos.daño;
        defensa = datos.defensa;
        penetracion = datos.penetracion;
        rango = datos.rango;
        tiempoEntreAtaques = Mathf.Max(0.05f, datos.tiempoEntreAtaques);
        buffArea = datos.buffArea;
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
