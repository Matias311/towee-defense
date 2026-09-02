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
    [Tooltip("Cantidad de torres aleatorias entregadas al comenzar la partida")]
    [Range(0, 3)]
    public int torresIniciales = 3;
    public int puntosActuales;
    public int torresObtenidas;
    public int torresEnInventario;
    [Tooltip("Cantidad de recompensas recientes en las que una torre no puede repetirse")]
    public int recompensasAntiRepeticion = 2;
    [Tooltip("Wave actual usada para ajustar el azar de las torres")]
    public int waveActual = 1;

    [Header("Validacion")]
    [Tooltip("Radio de espacio libre que necesita la torre para poder colocarse")]
    public float radioValidacion = 1f;

    [Tooltip("Capas que bloquean la colocacion (camino y estructuras)")]
    public LayerMask capasBloqueadas;

    [Tooltip("Bloquea la colocacion sobre el recorrido formado por los waypoints del spawner")]
    public bool bloquearCaminoPorWaypoints = true;
    [Tooltip("Ancho de seguridad del camino alrededor de los segmentos entre waypoints")]
    public float radioCamino = 1.5f;

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
    private GameObject torreSeleccionada;
    private readonly List<GameObject> inventarioTorres = new List<GameObject>();
    private readonly List<TowerType> tiposObtenidos = new List<TowerType>();
    private readonly List<TowerType> tiposRecientes = new List<TowerType>();
    private readonly List<GameObject> variantesAutomaticas = new List<GameObject>();

    void Start()
    {
        if (torresDisponibles.Count == 0 && towerPrefab != null)
        {
            torresDisponibles.Add(towerPrefab);
        }

        CrearVariantesAutomaticasSiHaceFalta();

        for (int i = 0; i < torresIniciales; i++)
        {
            ObtenerTorreAleatoria();
        }

        void CrearVariantesAutomaticasSiHaceFalta()
        {
            if (torresDisponibles.Count < 4) return;

            bool tieneTipoEspecializado = false;
            foreach (GameObject torre in torresDisponibles)
            {
                if (torre == null) continue;
                if (torre.GetComponent<TorreFrancotiradorStats>() != null
                    || torre.GetComponent<TorreAmetralladoraStats>() != null
                    || torre.GetComponent<TorreCanonStats>() != null)
                {
                    tieneTipoEspecializado = true;
                    break;
                }
            }

            if (tieneTipoEspecializado) return;

            GameObject plantilla = torresDisponibles[0];
            if (plantilla == null) return;

            TowerType[] tipos = {
                TowerType.Basica,
                TowerType.Francotirador,
                TowerType.Ametralladora,
                TowerType.Canon
            };
            List<GameObject> variantes = new List<GameObject>();

            foreach (TowerType tipo in tipos)
            {
                GameObject variante = Instantiate(plantilla);
                variante.name = $"{plantilla.name}_{tipo}";
                variante.SetActive(false);

                switch (tipo)
                {
                    case TowerType.Francotirador:
                        variante.AddComponent<TorreFrancotiradorStats>();
                        break;
                    case TowerType.Ametralladora:
                        variante.AddComponent<TorreAmetralladoraStats>();
                        break;
                    case TowerType.Canon:
                        variante.AddComponent<TorreCanonStats>();
                        break;
                }

                variantes.Add(variante);
                variantesAutomaticas.Add(variante);
            }

            torresDisponibles = variantes;
            Debug.Log("[TOWER] Se generaron variantes automaticas: Basica, Francotirador, Ametralladora y Canon.");
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
        inventarioTorres.Add(torre);
        torresEnInventario = inventarioTorres.Count;
        while (tiposRecientes.Count > recompensasAntiRepeticion)
        {
            tiposRecientes.RemoveAt(0);
        }

        torresObtenidas++;
        Debug.Log($"[TOWER] Torre obtenida: {torre.name}. Total: {torresObtenidas}. En inventario: {torresEnInventario}");
        if (!colocando) {
            EmpezarColocacion();
        }
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
        if (colocando || inventarioTorres.Count == 0) return;

        torreSeleccionada = inventarioTorres[0];
        colocando = true;
        previewTorre = Instantiate(torreSeleccionada);
        previewTorre.SetActive(true);
        PrepararVisualizadorRango(previewTorre);
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
        if (colisiones.Length > 0) return false;

        if (bloquearCaminoPorWaypoints && EstaSobreElCamino(posicion)) {
            return false;
        }

        return true;
    }

    bool EstaSobreElCamino(Vector3 posicion)
    {
        EnemySpawner spawner = FindAnyObjectByType<EnemySpawner>();
        if (spawner == null || spawner.waypoints == null || spawner.waypoints.Count < 2) {
            return false;
        }

        float radio = Mathf.Max(radioValidacion, radioCamino);
        float radioCuadrado = radio * radio;

        for (int i = 0; i < spawner.waypoints.Count - 1; i++) {
            Transform inicio = spawner.waypoints[i];
            Transform final = spawner.waypoints[i + 1];
            if (inicio == null || final == null) continue;

            Vector3 inicioPlano = new Vector3(inicio.position.x, posicion.y, inicio.position.z);
            Vector3 finalPlano = new Vector3(final.position.x, posicion.y, final.position.z);
            Vector3 segmento = finalPlano - inicioPlano;
            float longitudCuadrada = segmento.sqrMagnitude;
            float progreso = longitudCuadrada > 0f
                ? Mathf.Clamp01(Vector3.Dot(posicion - inicioPlano, segmento) / longitudCuadrada)
                : 0f;
            Vector3 puntoCercano = inicioPlano + segmento * progreso;

            if ((posicion - puntoCercano).sqrMagnitude <= radioCuadrado) {
                return true;
            }
        }

        return false;
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
            torreSeleccionada,
            previewTorre.transform.position,
            Quaternion.identity
        );
        torre.SetActive(true);
        TowerStats estadisticas = torre.GetComponent<TowerStats>();
        if (estadisticas == null)
        {
            estadisticas = torre.AddComponent<TowerStats>();
        }
        if (torre.GetComponent<TowerAttackController>() == null)
        {
            torre.AddComponent<TowerAttackController>();
        }
        PrepararVisualizadorRango(torre);
        ConfigurarTipoAutomatico(torre, estadisticas);
        estadisticas.Configurar(estadisticasTorre);
        inventarioTorres.RemoveAt(0);
        torresEnInventario = inventarioTorres.Count;

        // Destruir el preview actual y crear uno nuevo para seguir colocando torres
        Destroy(previewTorre);
        previewTorre = null;
        colocando = false;
        torreSeleccionada = null;
        if (inventarioTorres.Count > 0) {
            EmpezarColocacion();
        }
    }

    void PrepararVisualizadorRango(GameObject torre)
    {
        if (torre.GetComponent<TowerRangeVisualizer>() == null)
        {
            torre.AddComponent<TowerRangeVisualizer>();
        }
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