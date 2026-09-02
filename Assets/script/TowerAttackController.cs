using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TowerStats))]
public class TowerAttackController : MonoBehaviour {
    private TowerStats estadisticas;
    private float temporizadorAtaque;

    void Awake() {
        estadisticas = GetComponent<TowerStats>();
    }

    void Update() {
        temporizadorAtaque -= Time.deltaTime;
        if (temporizadorAtaque > 0f) return;

        EnemyStats objetivo = EncontrarObjetivo();
        if (objetivo == null) return;

        if (estadisticas.tipoAtaque == TowerAttackType.Area) {
            AtacarEnArea(objetivo.transform.position);
        } else {
            AtacarObjetivo(objetivo);
        }

        temporizadorAtaque = Mathf.Max(0.05f, estadisticas.tiempoEntreAtaques);
    }

    void AtacarObjetivo(EnemyStats objetivo) {
        objetivo.RecibirDanio(new DamageData(
            estadisticas.daño,
            DamageType.Fisico,
            estadisticas.penetracion
        ));
    }

    void AtacarEnArea(Vector3 posicionImpacto) {
        Collider[] colisiones = Physics.OverlapSphere(
            posicionImpacto,
            estadisticas.radioArea
        );
        HashSet<EnemyStats> objetivos = new HashSet<EnemyStats>();

        foreach (Collider colision in colisiones) {
            EnemyStats enemigo = colision.GetComponentInParent<EnemyStats>();
            if (enemigo != null && enemigo.vidaActual > 0f) {
                objetivos.Add(enemigo);
            }
        }

        int afectados = 0;
        foreach (EnemyStats enemigo in objetivos) {
            AtacarObjetivo(enemigo);
            afectados++;
            if (afectados >= estadisticas.cantidadObjetivos) break;
        }
    }

    EnemyStats EncontrarObjetivo() {
        Collider[] colisiones = Physics.OverlapSphere(
            transform.position,
            estadisticas.rango
        );
        EnemyStats objetivo = null;
        float mejorValor = estadisticas.prioridadObjetivo == TowerTargetPriority.MasCercano
            ? float.MaxValue
            : float.MinValue;

        foreach (Collider colision in colisiones) {
            EnemyStats enemigo = colision.GetComponentInParent<EnemyStats>();
            if (enemigo == null || enemigo.vidaActual <= 0f) continue;

            float distancia = (enemigo.transform.position - transform.position).sqrMagnitude;
            float valor = estadisticas.prioridadObjetivo == TowerTargetPriority.MasVida
                ? enemigo.vidaActual
                : distancia;
            bool elegir = estadisticas.prioridadObjetivo == TowerTargetPriority.MasCercano
                ? valor < mejorValor
                : valor > mejorValor;

            if (elegir) {
                mejorValor = valor;
                objetivo = enemigo;
            }
        }

        return objetivo;
    }
}
