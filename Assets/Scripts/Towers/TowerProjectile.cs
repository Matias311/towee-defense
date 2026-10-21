using System.Collections.Generic;
using UnityEngine;

public class TowerProjectile : MonoBehaviour
{
    [Tooltip("Giro del sprite: 0 si la imagen apunta a la derecha; -90 si apunta hacia arriba.")]
    public float offsetAnguloSprite;
    [Header("Visibilidad del disparo")]
    [Min(0.1f)] public float multiplicadorVisual = 1.5f;
    [Tooltip("Tamaño mínimo del sprite en unidades de mundo. No cambia el radio de impacto.")]
    [Min(0f)] public float tamanoMinimoSprite = 1.2f;
    public bool mostrarEstela = true;
    public Color colorEstela = new Color(1f, 0.8f, 0.25f, 1f);
    [Min(0.01f)] public float anchoEstela = 0.14f;
    [Min(0.01f)] public float duracionEstela = 0.16f;
    private SpriteRenderer sprite;
    private Camera camara;
    private Vector3 direccionVuelo;
    private EnemyStats objetivo;
    private DamageData daño;
    private PerfilImpactoPasiva perfilPasiva;
    private float radioArea;
    private int cantidadObjetivos;
    private float velocidad;
    private float tiempoVida = 8f;
    private Quaternion rotacionVisualLocal = Quaternion.identity;
    private float ralentizacionImpacto;
    private float duracionRalentizacion;
    private TowerType tipoTorre = TowerType.Basica;
    private Vector3 origenDisparo;

    void Awake()
    {
        rotacionVisualLocal = transform.localRotation;
        sprite = GetComponentInChildren<SpriteRenderer>();
        transform.localScale *= Mathf.Max(0.1f, multiplicadorVisual);
        if (sprite != null && sprite.sprite != null)
        {
            Vector3 dimensiones = sprite.sprite.bounds.size;
            Vector3 escala = sprite.transform.lossyScale;
            float tamano = Mathf.Max(dimensiones.x * Mathf.Abs(escala.x), dimensiones.y * Mathf.Abs(escala.y));
            if (tamano > 0.001f && tamano < tamanoMinimoSprite)
                sprite.transform.localScale *= tamanoMinimoSprite / tamano;
        }
    }

    public void Inicializar(
        EnemyStats objetivoInicial,
        DamageData dañoInicial,
        float radioAreaInicial,
        int cantidadObjetivosInicial,
        float velocidadInicial,
        PerfilImpactoPasiva perfil = default,
        float porcentajeRalentizacion = 0f,
        float duracionRalentizacion = 0f,
        TowerType tipo = TowerType.Basica,
        Vector3 origen = default
    )
    {
        rotacionVisualLocal = transform.localRotation;
        objetivo = objetivoInicial;
        tiempoVida = 8f;
        daño = dañoInicial;
        perfilPasiva = perfil;
        ralentizacionImpacto = porcentajeRalentizacion;
        this.duracionRalentizacion = duracionRalentizacion;
        tipoTorre = tipo;
        origenDisparo = origen;
        radioArea = Mathf.Max(0f, radioAreaInicial);
        cantidadObjetivos = Mathf.Max(1, cantidadObjetivosInicial);
        velocidad = Mathf.Max(0.1f, velocidadInicial);
        Color colorDisparo = tipo == TowerType.Ametralladora ? new Color(0.22f, 0.95f, 1f)
            : tipo == TowerType.Francotirador ? new Color(1f, 0.88f, 0.35f)
            : colorEstela;
        if (sprite != null) sprite.color = colorDisparo;
        if (mostrarEstela && GetComponentInChildren<TrailRenderer>() == null)
        {
            float ancho = tipo == TowerType.Ametralladora ? anchoEstela * 1.25f : anchoEstela;
            float estela = tipo == TowerType.Ametralladora ? duracionEstela * 1.7f : duracionEstela * 1.25f;
            TowerShotTrail.Crear(transform, colorDisparo, ancho, estela);
        }
        // Dar tiempo al proyectil para alcanzar objetivos lejanos sin alterar su velocidad configurada.
        if (objetivo != null) tiempoVida = Mathf.Max(8f,
            Vector3.Distance(transform.position, ObtenerPosicionObjetivo(objetivo)) / velocidad + 2f);
    }

    void Update()
    {
        tiempoVida -= Time.deltaTime;
        if (tiempoVida <= 0f || objetivo == null || objetivo.vidaActual <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 posicionObjetivo = ObtenerPosicionObjetivo(objetivo);
        float distanciaImpacto = ObtenerRadioImpacto(objetivo);
        Vector3 direccion = posicionObjetivo - transform.position;
        direccionVuelo = direccion;
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

        if ((posicionObjetivo - transform.position).sqrMagnitude <= distanciaImpacto * distanciaImpacto)
        {
            Impactar();
            return;
        }

        if (sprite == null && direccion.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direccion.normalized)
                * rotacionVisualLocal;
        }
    }

    void LateUpdate()
    {
        if (sprite == null) return;
        if (camara == null || !camara.isActiveAndEnabled) camara = Camera.main;
        if (camara == null) return;

        // Mantener el sprite en el plano de la camara y apuntarlo hacia su trayectoria.
        float x = Vector3.Dot(direccionVuelo, camara.transform.right);
        float y = Vector3.Dot(direccionVuelo, camara.transform.up);
        float angulo = Mathf.Atan2(y, x) * Mathf.Rad2Deg + offsetAnguloSprite;
        sprite.transform.rotation = camara.transform.rotation * Quaternion.Euler(0f, 0f, angulo);
    }

    Vector3 ObtenerPosicionObjetivo(EnemyStats enemigo)
    {
        Collider colision = enemigo.GetComponentInChildren<Collider>();
        if (colision != null) return colision.bounds.center;
        Renderer renderizador = enemigo.GetComponentInChildren<Renderer>();
        return renderizador != null ? renderizador.bounds.center : enemigo.transform.position;
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
        Vector3 posicionImpacto = ObtenerPosicionObjetivo(objetivo);
        Collider colisionObjetivo = objetivo.GetComponentInChildren<Collider>();
        Renderer visualObjetivo = objetivo.GetComponentInChildren<Renderer>();
        float separacionVisual = colisionObjetivo != null
            ? colisionObjetivo.bounds.extents.magnitude + 0.1f
            : visualObjetivo != null ? visualObjetivo.bounds.extents.magnitude + 0.1f : 0.3f;
        TowerImpactVisual.Mostrar(posicionImpacto, radioArea, separacionVisual, tipoTorre, origenDisparo);
        if (radioArea <= 0f)
        {
            AplicarImpacto(objetivo);
            Destroy(gameObject);
            return;
        }

        Collider[] colisiones = Physics.OverlapSphere(posicionImpacto, radioArea);
        HashSet<EnemyStats> objetivos = new HashSet<EnemyStats>();
        objetivos.Add(objetivo);
        AplicarImpacto(objetivo);
        int afectados = 1;
        foreach (Collider colision in colisiones)
        {
            EnemyStats enemigo = colision.GetComponentInParent<EnemyStats>();
            if (enemigo != null && enemigo.vidaActual > 0f)
            {
                objetivos.Add(enemigo);
            }
        }

        foreach (EnemyStats enemigo in objetivos)
        {
            if (afectados >= cantidadObjetivos) break;
            if (enemigo == objetivo) continue;
            AplicarImpacto(enemigo);
            afectados++;
            if (afectados >= cantidadObjetivos) break;
        }

        Destroy(gameObject);
    }

    void AplicarImpacto(EnemyStats enemigo)
    {
        if (!SistemaPasivas.AplicarImpactoConResultado(enemigo, daño, perfilPasiva, true)) return;
        if (ralentizacionImpacto <= 0f) return;
        EnemyMovement movimiento = enemigo.GetComponent<EnemyMovement>();
        if (movimiento != null) movimiento.AplicarRalentizacion(ralentizacionImpacto, duracionRalentizacion);
    }
}
