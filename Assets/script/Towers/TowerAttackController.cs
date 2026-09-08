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
            if (estadisticas.usarProyectil) {
                LanzarProyectil(objetivo);
            } else {
                AtacarEnArea(objetivo.transform.position);
            }
        } else {
            if (estadisticas.usarProyectil) {
                LanzarProyectil(objetivo);
            } else {
                AtacarObjetivo(objetivo);
            }
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

    void LanzarProyectil(EnemyStats objetivo) {
        GameObject objetoProyectil = estadisticas.prefabProyectil != null
            ? Instantiate(estadisticas.prefabProyectil)
            : CrearProyectilVisual();
        objetoProyectil.transform.position = transform.position;

        TowerProjectile proyectil = objetoProyectil.GetComponent<TowerProjectile>();
        if (proyectil == null) {
            proyectil = objetoProyectil.AddComponent<TowerProjectile>();
        }

        proyectil.Inicializar(
            objetivo,
            new DamageData(
                estadisticas.daño,
                DamageType.Fisico,
                estadisticas.penetracion
            ),
            estadisticas.tipoAtaque == TowerAttackType.Area
                ? estadisticas.radioArea
                : 0f,
            estadisticas.cantidadObjetivos,
            estadisticas.velocidadProyectil
        );
    }

    GameObject CrearProyectilVisual() {
        GameObject proyectil = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        proyectil.name = "ProyectilTorre";
        proyectil.transform.position = transform.position;
        proyectil.transform.localScale = Vector3.one * 0.18f;

        Collider colision = proyectil.GetComponent<Collider>();
        if (colision != null) {
            Destroy(colision);
        }

        Renderer renderizador = proyectil.GetComponent<Renderer>();
        renderizador.material = CrearMaterialProyectil();
        return proyectil;
    }

    Material CrearMaterialProyectil() {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) shader = Shader.Find("Unlit/Color");
        Material material = new Material(shader);
        material.color = new Color(1f, 0.72f, 0.15f, 1f);
        return material;
    }

    EnemyStats EncontrarObjetivo() {
        EnemyStats objetivo = null;
        float mejorValor = estadisticas.prioridadObjetivo == TowerTargetPriority.MasCercano
            ? float.MaxValue
            : float.MinValue;

        EnemyStats[] enemigos = FindObjectsByType<EnemyStats>(FindObjectsSortMode.None);
        foreach (EnemyStats enemigo in enemigos) {
            if (enemigo == null || enemigo.vidaActual <= 0f) continue;

            float distancia = (enemigo.transform.position - transform.position).sqrMagnitude;
            if (distancia > estadisticas.rango * estadisticas.rango) continue;

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
