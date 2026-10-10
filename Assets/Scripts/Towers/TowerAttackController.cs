using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TowerStats))]
public class TowerAttackController : MonoBehaviour {
    [Header("Salida del disparo")]
    [Tooltip("Punto en la boca del cañon. Si queda vacio, se usa la parte superior del modelo.")]
    public Transform puntoDisparo;
    private Renderer[] renderizadores;
    private Material materialProyectil;
    private TowerStats estadisticas;
    private float temporizadorAtaque;

    void Awake() {
        estadisticas = GetComponent<TowerStats>();
        renderizadores = GetComponentsInChildren<Renderer>();
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

    void AtacarObjetivo(EnemyStats objetivo, bool mostrarImpacto = true) {
        if (mostrarImpacto) {
            Collider colision = objetivo.GetComponentInChildren<Collider>();
            Renderer visual = objetivo.GetComponentInChildren<Renderer>();
            Vector3 centro = colision != null ? colision.bounds.center
                : visual != null ? visual.bounds.center : objetivo.transform.position;
            float separacion = colision != null ? colision.bounds.extents.magnitude + 0.1f
                : visual != null ? visual.bounds.extents.magnitude + 0.1f : 0.3f;
            TowerImpactVisual.Mostrar(centro, 0f, separacion);
        }
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
            AtacarObjetivo(enemigo, false);
            afectados++;
            if (afectados >= estadisticas.cantidadObjetivos) break;
        }
        if (afectados > 0) TowerImpactVisual.Mostrar(posicionImpacto, estadisticas.radioArea);
    }

    void LanzarProyectil(EnemyStats objetivo) {
        GameObject objetoProyectil = estadisticas.prefabProyectil != null
            ? Instantiate(estadisticas.prefabProyectil)
            : CrearProyectilVisual();
        objetoProyectil.transform.position = ObtenerPosicionDisparo();
        objetoProyectil.SetActive(true);

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

    Vector3 ObtenerCentroTorre() {
        TowerRangeVisualizer rango = GetComponent<TowerRangeVisualizer>();
        return rango != null ? rango.ObtenerCentroVisualWorld() : transform.position;
    }

    Vector3 ObtenerPosicionDisparo() {
        if (puntoDisparo != null) return puntoDisparo.position;
        Bounds limites = new Bounds();
        bool encontrado = false;
        foreach (Renderer renderizador in renderizadores) {
            if (renderizador == null || renderizador is LineRenderer || renderizador is TrailRenderer) continue;
            if (!encontrado) limites = renderizador.bounds;
            else limites.Encapsulate(renderizador.bounds);
            encontrado = true;
        }
        return encontrado
            ? new Vector3(limites.center.x, limites.max.y + 0.15f, limites.center.z)
            : transform.position + Vector3.up;
    }

    void OnDestroy() {
        if (materialProyectil != null) Destroy(materialProyectil);
    }

    Material CrearMaterialProyectil() {
        if (materialProyectil != null) return materialProyectil;
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) shader = Shader.Find("Unlit/Color");
        Material material = new Material(shader);
        material.color = new Color(1f, 0.72f, 0.15f, 1f);
        materialProyectil = material;
        return material;
    }

    EnemyStats EncontrarObjetivo() {
        EnemyStats objetivo = null;
        float mejorValor = estadisticas.prioridadObjetivo == TowerTargetPriority.MasCercano
            ? float.MaxValue
            : float.MinValue;

        Vector3 centroTorre = ObtenerCentroTorre();
        EnemyStats[] enemigos = FindObjectsByType<EnemyStats>(FindObjectsSortMode.None);
        foreach (EnemyStats enemigo in enemigos) {
            if (enemigo == null || enemigo.vidaActual <= 0f) continue;

            float distancia = (enemigo.transform.position - centroTorre).sqrMagnitude;
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
