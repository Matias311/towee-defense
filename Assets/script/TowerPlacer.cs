using System.Collections.Generic;
using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    [Header("Prefab de la torre")]
    public GameObject towerPrefab;
    [Tooltip("Tipos de torres posibles como recompensa")]
    public List<GameObject> torresDisponibles = new List<GameObject>();
    [Tooltip("Estadisticas que usaran automaticamente todas las torres colocadas")]
    public TowerStatsData estadisticasTorre;

    [Header("Recompensas")]
    [Tooltip("Puntos necesarios para obtener una torre aleatoria")]
    public int puntosPorTorre = 24;
    public int puntosActuales;
    public int torresObtenidas;
    [Tooltip("Cantidad de recompensas recientes en las que una torre no puede repetirse")]
    public int recompensasAntiRepeticion = 2;
    [Tooltip("Wave actual usada para ajustar el azar de las torres")]
    public int waveActual = 1;

    [Header("Validacion")]
    [Tooltip("Radio de espacio libre que necesita la torre para poder colocarse")]
    public float radioValidacion = 1f;

    [Tooltip("Capas que bloquean la colocacion (camino y estructuras)")]
    public LayerMask capasBloqueadas;

    [Header("Referencias")]
    public Camera camaraPrincipal;

    [Header("Materiales de preview (opcional)")]
    public Material materialValido;
    public Material materialInvalido;

    [Header("Modo de prueba")]
    [Tooltip("Si esta tildado, el modo colocacion arranca activo apenas empieza el juego, sin necesitar boton de UI")]
    public bool activarAlIniciar = true;

    private GameObject previewTorre;
    private bool colocando = false;
    private readonly List<TowerType> tiposObtenidos = new List<TowerType>();
    private readonly List<TowerType> tiposRecientes = new List<TowerType>();

    void Start()
    {
        if (torresDisponibles.Count == 0 && towerPrefab != null)
        {
            torresDisponibles.Add(towerPrefab);
        }

        if (camaraPrincipal == null)
        {
            camaraPrincipal = Camera.main;
        }

        if (activarAlIniciar)
        {
            EmpezarColocacion();
        }
    }

    public void RegistrarPuntos(int puntos)
    {
        if (puntos <= 0 || puntosPorTorre <= 0) return;

        puntosActuales += puntos;
        while (puntosActuales >= puntosPorTorre)
        {
            puntosActuales -= puntosPorTorre;
            ObtenerTorreAleatoria();
        }
    }

    public void ActualizarWave(int wave)
    {
        waveActual = Mathf.Max(1, wave);
    }

    void ObtenerTorreAleatoria()
    {
        if (torresDisponibles.Count == 0)
        {
            Debug.LogWarning("[TOWER] No hay torres disponibles para entregar.");
            return;
        }

        List<GameObject> candidatas = ObtenerCandidatas();
        GameObject torre = SeleccionarTorrePonderada(candidatas);
        if (torre == null) return;

        TowerType tipo = ObtenerTipoTorre(torre);
        tiposObtenidos.Add(tipo);
        tiposRecientes.Add(tipo);
        while (tiposRecientes.Count > recompensasAntiRepeticion)
        {
            tiposRecientes.RemoveAt(0);
        }

        torresObtenidas++;
        Debug.Log($"[TOWER] Torre obtenida: {torre.name}. Total: {torresObtenidas}");
    }

    List<GameObject> ObtenerCandidatas()
    {
        List<TowerType> faltantes = new List<TowerType>();
        foreach (GameObject torre in torresDisponibles)
        {
            if (torre == null) continue;

            TowerType tipo = ObtenerTipoTorre(torre);
            if (!tiposObtenidos.Contains(tipo) && !faltantes.Contains(tipo))
            {
                faltantes.Add(tipo);
            }
        }

        if (faltantes.Count > 0 && torresObtenidas % 3 == 2)
        {
            List<GameObject> garantizadas = new List<GameObject>();
            foreach (GameObject torre in torresDisponibles)
            {
                if (torre != null && faltantes.Contains(ObtenerTipoTorre(torre)))
                {
                    garantizadas.Add(torre);
                }
            }
            return garantizadas;
        }

        if (torresObtenidas == 0)
        {
            List<GameObject> iniciales = new List<GameObject>();
            foreach (GameObject torre in torresDisponibles)
            {
                TowerType tipo = ObtenerTipoTorre(torre);
                if (torre != null && (tipo == TowerType.Basica || tipo == TowerType.Ametralladora))
                {
                    iniciales.Add(torre);
                }
            }

            if (iniciales.Count > 0) return iniciales;
        }

        return torresDisponibles;
    }

    GameObject SeleccionarTorrePonderada(List<GameObject> candidatas)
    {
        float total = 0f;
        foreach (GameObject torre in candidatas)
        {
            if (torre == null || tiposRecientes.Contains(ObtenerTipoTorre(torre))) continue;
            total += ObtenerPesoTorre(torre);
        }

        if (total <= 0f)
        {
            foreach (GameObject torre in candidatas)
            {
                if (torre != null) total += ObtenerPesoTorre(torre);
            }
        }

        if (total <= 0f) return null;

        float valor = Random.value * total;
        foreach (GameObject torre in candidatas)
        {
            if (torre == null || tiposRecientes.Contains(ObtenerTipoTorre(torre))) continue;

            valor -= ObtenerPesoTorre(torre);
            if (valor <= 0f) return torre;
        }

        for (int i = candidatas.Count - 1; i >= 0; i--)
        {
            if (candidatas[i] != null) return candidatas[i];
        }

        return null;
    }

    float ObtenerPesoTorre(GameObject torre)
    {
        TowerType tipo = ObtenerTipoTorre(torre);
        float progreso = Mathf.Clamp01((waveActual - 1) / 14f);

        switch (tipo)
        {
            case TowerType.Basica:
                return Mathf.Lerp(40f, 15f, progreso);
            case TowerType.Ametralladora:
                return Mathf.Lerp(30f, 20f, progreso);
            case TowerType.Francotirador:
            case TowerType.Canon:
                return Mathf.Lerp(15f, 32.5f, progreso);
            default:
                return 1f;
        }
    }

    TowerType ObtenerTipoTorre(GameObject torre)
    {
        TorreFrancotiradorStats francotirador = torre.GetComponent<TorreFrancotiradorStats>();
        if (francotirador != null) return TowerType.Francotirador;

        TorreAmetralladoraStats ametralladora = torre.GetComponent<TorreAmetralladoraStats>();
        if (ametralladora != null) return TowerType.Ametralladora;

        TorreCanonStats canon = torre.GetComponent<TorreCanonStats>();
        if (canon != null) return TowerType.Canon;

        return TowerType.Basica;
    }

    void Update()
    {
        if (colocando)
        {
            ActualizarPreview();

            if (Input.GetMouseButtonDown(0))
            {
                IntentarColocarTorre();
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelarColocacion();
            }
        }
    }

    // Llamar esta funcion desde un boton de UI para empezar a colocar una torre
    public void EmpezarColocacion()
    {
        if (towerPrefab == null) return;

        colocando = true;
        previewTorre = Instantiate(towerPrefab);
        // Desactivar collider del preview para que no interfiera con el raycast
        Collider col = previewTorre.GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    void ActualizarPreview()
    {
        if (previewTorre == null) return;

        Ray ray = camaraPrincipal.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            previewTorre.transform.position = hit.point;

            bool valido = EsPosicionValida(hit.point);
            PintarPreview(valido);
        }
    }

    bool EsPosicionValida(Vector3 posicion)
    {
        // Si hay algo (camino o estructura) dentro del radio, no es valido
        Collider[] colisiones = Physics.OverlapSphere(posicion, radioValidacion, capasBloqueadas);
        return colisiones.Length == 0;
    }

    void PintarPreview(bool valido)
    {
        Renderer rend = previewTorre.GetComponent<Renderer>();
        if (rend == null) return;

        if (valido && materialValido != null)
        {
            rend.material = materialValido;
        }
        else if (!valido && materialInvalido != null)
        {
            rend.material = materialInvalido;
        }
    }

    void IntentarColocarTorre()
    {
        if (!EsPosicionValida(previewTorre.transform.position))
        {
            // No se puede colocar aca, no hacemos nada (el preview sigue activo)
            return;
        }

        // Confirmar colocacion: instanciar la torre real
        GameObject torre = Instantiate(
            towerPrefab,
            previewTorre.transform.position,
            Quaternion.identity
        );
        TowerStats estadisticas = torre.GetComponent<TowerStats>();
        if (estadisticas == null)
        {
            estadisticas = torre.AddComponent<TowerStats>();
        }
        ConfigurarTipoAutomatico(torre, estadisticas);
        estadisticas.Configurar(estadisticasTorre);

        // Destruir el preview actual y crear uno nuevo para seguir colocando torres
        Destroy(previewTorre);
        previewTorre = Instantiate(towerPrefab);
        Collider col = previewTorre.GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    void ConfigurarTipoAutomatico(GameObject torre, TowerStats estadisticas)
    {
        if (torre.GetComponent<TorreFrancotiradorStats>() != null)
        {
            estadisticas.tipo = TowerType.Francotirador;
        }
        else if (torre.GetComponent<TorreAmetralladoraStats>() != null)
        {
            estadisticas.tipo = TowerType.Ametralladora;
        }
        else if (torre.GetComponent<TorreCanonStats>() != null)
        {
            estadisticas.tipo = TowerType.Canon;
        }
        else if (torre.GetComponent<TorreBasicaStats>() != null)
        {
            estadisticas.tipo = TowerType.Basica;
        }
    }

    void CancelarColocacion()
    {
        if (previewTorre != null)
        {
            Destroy(previewTorre);
        }
        colocando = false;
    }
}