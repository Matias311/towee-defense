using System.Collections.Generic;
using UnityEngine;

public class TowerProjectile : MonoBehaviour
{
    private EnemyStats objetivo;
    private DamageData daño;
    private float radioArea;
    private int cantidadObjetivos;
    private float velocidad;
    private float tiempoVida = 8f;
    private Quaternion rotacionVisualLocal = Quaternion.identity;

    void Awake()
    {
        rotacionVisualLocal = transform.localRotation;
    }

    public void Inicializar(
        EnemyStats objetivoInicial,
        DamageData dañoInicial,
        float radioAreaInicial,
        int cantidadObjetivosInicial,
        float velocidadInicial
    )
    {
        rotacionVisualLocal = transform.localRotation;
        objetivo = objetivoInicial;
        daño = dañoInicial;
        radioArea = Mathf.Max(0f, radioAreaInicial);
        cantidadObjetivos = Mathf.Max(1, cantidadObjetivosInicial);
        EnemyMovement movimiento = objetivoInicial != null
            ? objetivoInicial.GetComponent<EnemyMovement>()
            : null;
        float velocidadEnemigo = movimiento != null ? movimiento.speed : 0f;
        velocidad = Mathf.Max(0.1f, velocidadInicial, velocidadEnemigo + 8f);
    }

    void Update()
    {
        tiempoVida -= Time.deltaTime;
        if (tiempoVida <= 0f || objetivo == null || objetivo.vidaActual <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 posicionObjetivo = objetivo.transform.position;
        float distanciaImpacto = ObtenerRadioImpacto(objetivo);
        Vector3 direccion = posicionObjetivo - transform.position;
        if (direccion.sqrMagnitude <= distanciaImpacto * distanciaImpacto)
        {
            Impactar();
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            posicionObjetivo,
            velocidad * Time.deltaTime
        );

        if (direccion.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direccion.normalized)
                * rotacionVisualLocal;
        }
    }

    float ObtenerRadioImpacto(EnemyStats enemigo)
    {
        Collider colision = enemigo.GetComponentInChildren<Collider>();
        if (colision != null)
        {
            return Mathf.Max(0.2f, colision.bounds.extents.magnitude * 0.35f);
        }

        Renderer renderizador = enemigo.GetComponentInChildren<Renderer>();
        if (renderizador != null)
        {
            return Mathf.Max(0.2f, renderizador.bounds.extents.magnitude * 0.35f);
        }

        return 0.2f;
    }

    void Impactar()
    {
        Vector3 posicionImpacto = objetivo.transform.position;
        if (radioArea <= 0f)
        {
            objetivo.RecibirDanio(daño);
            Destroy(gameObject);
            return;
        }

        Collider[] colisiones = Physics.OverlapSphere(posicionImpacto, radioArea);
        HashSet<EnemyStats> objetivos = new HashSet<EnemyStats>();
        foreach (Collider colision in colisiones)
        {
            EnemyStats enemigo = colision.GetComponentInParent<EnemyStats>();
            if (enemigo != null && enemigo.vidaActual > 0f)
            {
                objetivos.Add(enemigo);
            }
        }

        int afectados = 0;
        foreach (EnemyStats enemigo in objetivos)
        {
            enemigo.RecibirDanio(daño);
            afectados++;
            if (afectados >= cantidadObjetivos) break;
        }

        Destroy(gameObject);
    }
}
