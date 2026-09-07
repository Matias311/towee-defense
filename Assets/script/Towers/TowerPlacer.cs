using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerPrefabEntry
{
    public TowerType tipo = TowerType.Basica;
    public GameObject prefab;
}

public class TowerPlacer : MonoBehaviour
{
    [Tooltip("Tipos de torres posibles como recompensa")]
    public List<TowerPrefabEntry> torresDisponibles = new List<TowerPrefabEntry>();
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
    [Tooltip("Giro horizontal adicional del modelo si su frente no apunta hacia Z positivo")]
    public float offsetRotacionTorre;

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
    private readonly Dictionary<GameObject, TowerType> tiposPorPrefab = new Dictionary<GameObject, TowerType>();

    void Start()
    {
        ConstruirMapaDeTipos();

        for (int i = 0; i < torresIniciales; i++)
        {
            ObtenerTorreAleatoria();
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

    void ConstruirMapaDeTipos()
    {
        tiposPorPrefab.Clear();
        foreach (TowerPrefabEntry entrada in torresDisponibles)
        {
            if (entrada == null || entrada.prefab == null) continue;
            if (!tiposPorPrefab.ContainsKey(entrada.prefab))
            {
                tiposPorPrefab.Add(entrada.prefab, entrada.tipo);
            }
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
        if (tiposPorPrefab.Count == 0)
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
        if (!colocando)
        {
            EmpezarColocacion();
        }
    }

    List<GameObject> ObtenerCandidatas()
    {
        List<TowerType> faltantes = new List<TowerType>();
        foreach (TowerPrefabEntry entrada in torresDisponibles)
        {
            if (entrada == null || entrada.prefab == null) continue;
            if (!tiposObtenidos.Contains(entrada.tipo) && !faltantes.Contains(entrada.tipo))
            {
                faltantes.Add(entrada.tipo);
            }
        }

        if (faltantes.Count > 0 && torresObtenidas % 3 == 2)
        {
            List<GameObject> garantizadas = new List<GameObject>();
            foreach (TowerPrefabEntry entrada in torresDisponibles)
            {
                if (entrada != null && entrada.prefab != null && faltantes.Contains(entrada.tipo))
                {
                    garantizadas.Add(entrada.prefab);
                }
            }
            return garantizadas;
        }

        if (torresObtenidas == 0)
        {
            List<GameObject> iniciales = new List<GameObject>();
            foreach (TowerPrefabEntry entrada in torresDisponibles)
            {
                if (entrada != null && entrada.prefab != null
                    && (entrada.tipo == TowerType.Basica || entrada.tipo == TowerType.Ametralladora))
                {
                    iniciales.Add(entrada.prefab);
                }
            }
            if (iniciales.Count > 0) return iniciales;
        }

        List<GameObject> disponibles = new List<GameObject>();
        foreach (TowerPrefabEntry entrada in torresDisponibles)
        {
            if (entrada != null && entrada.prefab != null)
            {
                disponibles.Add(entrada.prefab);
            }
        }
        return disponibles;
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
        if (torre != null && tiposPorPrefab.TryGetValue(torre, out TowerType tipoConfigurado))
        {
            return tipoConfigurado;
        }

        TowerStats stats = torre != null ? torre.GetComponent<TowerStats>() : null;
        if (stats != null) return stats.tipo;

        if (torre == null) return TowerType.Basica;

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

            // Escape lo gestiona PauseMenu; cancelar con click derecho evita que
            // abrir el menu destruya el preview de la torre seleccionada.
            if (Input.GetMouseButtonDown(1))
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
        previewTorre.GetComponent<TowerRangeVisualizer>().SetVisible(true);
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
            TowerRangeVisualizer visualizador = previewTorre.GetComponent<TowerRangeVisualizer>();
            previewTorre.transform.rotation = ObtenerRotacionHaciaCamino(hit.point);
            visualizador.ColocarCentroVisualEn(hit.point);

            bool valido = EsPosicionValida(hit.point);
            PintarPreview(valido);
        }
    }

    bool EsPosicionValida(Vector3 posicion)
    {
        // Si hay algo (camino o estructura) dentro del radio, no es valido
        Collider[] colisiones = Physics.OverlapSphere(posicion, radioValidacion, capasBloqueadas);
        if (colisiones.Length > 0) return false;

        if (HayTorreEnLaPosicion(posicion)) {
            return false;
        }

        if (bloquearCaminoPorWaypoints && EstaSobreElCamino(posicion)) {
            return false;
        }

        return true;
    }

    bool HayTorreEnLaPosicion(Vector3 posicion)
    {
        Collider[] colisiones = Physics.OverlapSphere(
            posicion,
            radioValidacion,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        foreach (Collider colision in colisiones)
        {
            if (colision.GetComponentInParent<TowerStats>() != null)
            {
                return true;
            }
        }

        return false;
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
        TowerRangeVisualizer visualizador = previewTorre.GetComponent<TowerRangeVisualizer>();
        Vector3 centroTorre = visualizador.ObtenerCentroVisualWorld();
        if (!EsPosicionValida(centroTorre))
        {
            // No se puede colocar aca, no hacemos nada (el preview sigue activo)
            return;
        }

        // Confirmar colocacion: instanciar la torre real
        GameObject torre = Instantiate(
            torreSeleccionada,
            previewTorre.transform.position,
            previewTorre.transform.rotation
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
        torre.GetComponent<TowerRangeVisualizer>().SetVisible(false);
        if (torre.GetComponent<TowerHoverInfo>() == null)
        {
            torre.AddComponent<TowerHoverInfo>();
        }
        estadisticas.Configurar(estadisticasTorre);
        ConfigurarTipoAutomatico(torre, estadisticas);
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
        estadisticas.tipo = ObtenerTipoTorre(torreSeleccionada);
    }

    Quaternion ObtenerRotacionHaciaCamino(Vector3 posicionTorre)
    {
        EnemySpawner spawner = FindAnyObjectByType<EnemySpawner>();
        if (spawner == null || spawner.waypoints == null || spawner.waypoints.Count < 2)
        {
            return Quaternion.Euler(0f, offsetRotacionTorre, 0f);
        }

        Vector3 puntoObjetivo = spawner.waypoints[0] != null
            ? spawner.waypoints[0].position
            : posicionTorre + Vector3.forward;
        float distanciaMinima = float.MaxValue;

        for (int i = 0; i < spawner.waypoints.Count - 1; i++)
        {
            Transform inicio = spawner.waypoints[i];
            Transform final = spawner.waypoints[i + 1];
            if (inicio == null || final == null) continue;

            Vector3 inicioPlano = new Vector3(inicio.position.x, posicionTorre.y, inicio.position.z);
            Vector3 finalPlano = new Vector3(final.position.x, posicionTorre.y, final.position.z);
            Vector3 segmento = finalPlano - inicioPlano;
            float longitudCuadrada = segmento.sqrMagnitude;
            float progreso = longitudCuadrada > 0f
                ? Mathf.Clamp01(Vector3.Dot(posicionTorre - inicioPlano, segmento) / longitudCuadrada)
                : 0f;
            Vector3 puntoCercano = inicioPlano + segmento * progreso;
            float distancia = (posicionTorre - puntoCercano).sqrMagnitude;

            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                puntoObjetivo = puntoCercano;
            }
        }

        Vector3 direccion = puntoObjetivo - posicionTorre;
        direccion.y = 0f;
        if (direccion.sqrMagnitude < 0.0001f)
        {
            direccion = Vector3.forward;
        }

        return Quaternion.LookRotation(direccion.normalized, Vector3.up)
            * Quaternion.Euler(0f, offsetRotacionTorre, 0f);
    }

    void CancelarColocacion()
    {
        if (previewTorre != null)
        {
            Destroy(previewTorre);
        }
        previewTorre = null;
        colocando = false;
        torreSeleccionada = null;
    }
}