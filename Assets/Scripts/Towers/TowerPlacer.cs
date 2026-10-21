using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class TowerPrefabEntry
{
    public TowerType tipo = TowerType.Basica;
    public GameObject prefab;
    [Tooltip("Estadisticas propias de esta torreta. Si queda vacio, usa las estadisticas globales.")]
    public TowerStatsData estadisticas;
}

public class TowerPlacer : MonoBehaviour
{
    [Tooltip("Modelos posibles en las ofertas de la tienda")]
    public List<TowerPrefabEntry> torresDisponibles = new List<TowerPrefabEntry>();
    [Tooltip("Estadisticas que usaran automaticamente todas las torretas colocadas")]
    public TowerStatsData estadisticasTorre;

    [Header("Probabilidades y bonos de las torretas")]
    [Tooltip("Porcentajes relativos normalizados. Las reglas de primera recompensa, variedad y anti repetición filtran los tipos elegibles.")]
    public ConfiguracionTorretas balanceTorretas = new ConfiguracionTorretas();

    [Header("Tienda y engranajes")]
    public PreciosTorretas preciosTorretas = new PreciosTorretas();
    [Tooltip("Saldo mínimo inicial. Al comenzar se asegura el costo de las tres ofertas más baratas.")]
    public int puntosActuales = 150;
    [Range(0, 100), Tooltip("Porcentaje del precio efectivamente pagado que devuelve vender con V.")]
    public float porcentajeDevolucionVenta = 50;
    public int torresObtenidas;
    public int torresEnInventario;
    [Tooltip("Cantidad de recompensas recientes en las que una torreta no puede repetirse")]
    public int recompensasAntiRepeticion = 2;
    [Tooltip("Wave actual usada para ajustar el azar de las torretas")]
    public int waveActual = 1;

    [Header("Validacion")]
    [Tooltip("Radio de espacio libre que necesita la torreta para poder colocarse")]
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

    private GameObject previewTorre;
    private bool colocando = false;
    private GameObject torreSeleccionada;
    private readonly List<CartaTorreta> inventarioTorres = new List<CartaTorreta>();
    public IReadOnlyList<CartaTorreta> Cartas => inventarioTorres;
    public const int MaximoCartasVisibles = 4;
    public int CantidadCartasVisibles => Mathf.Min(MaximoCartasVisibles, inventarioTorres.Count);

    public bool EsCartaVisible(CartaTorreta carta)
    {
        int indice = inventarioTorres.IndexOf(carta);
        return indice >= 0 && indice < CantidadCartasVisibles;
    }
    public CartaTorreta CartaSeleccionada { get; private set; }
    public event Action AlCambiarCartas;
    public event Action<int> AlCambiarPuntos;
    private bool previewSobreTerreno;
    private int ultimoFrameCancelacion = -1;
    private int ultimoFrameSeleccion = -1;
    private readonly List<TowerType> tiposObtenidos = new List<TowerType>();
    private readonly List<TowerType> tiposRecientes = new List<TowerType>();
    private readonly Dictionary<GameObject, TowerType> tiposPorPrefab = new Dictionary<GameObject, TowerType>();
    private readonly Dictionary<GameObject, TowerStatsData> estadisticasPorPrefab =
        new Dictionary<GameObject, TowerStatsData>();

    void Start()
    {
        ConstruirMapaDeTipos();

        ReponerOfertas();
        // Financiar tres de las ofertas reales, aunque tengan rarezas distintas.
        puntosActuales = Mathf.Max(puntosActuales, CostoInicialTresTorretas(inventarioTorres));
        AlCambiarPuntos?.Invoke(puntosActuales);

        if (camaraPrincipal == null)
        {
            ResolverCamaraDeColocacion();
        }

        AlCambiarCartas?.Invoke();
    }

    public static int CostoInicialTresTorretas(IReadOnlyList<CartaTorreta> ofertas)
    {
        var precios = new List<int>();
        foreach (var oferta in ofertas) precios.Add(oferta.Precio);
        precios.Sort();
        int total = 0;
        for (int i = 0; i < Mathf.Min(3, precios.Count); i++) total += precios[i];
        return total;
    }

    void ConstruirMapaDeTipos()
    {
        tiposPorPrefab.Clear();
        estadisticasPorPrefab.Clear();
        foreach (TowerPrefabEntry entrada in torresDisponibles)
        {
            if (entrada == null || entrada.prefab == null) continue;
            if (!tiposPorPrefab.ContainsKey(entrada.prefab))
            {
                tiposPorPrefab.Add(entrada.prefab, entrada.tipo);
                estadisticasPorPrefab.Add(entrada.prefab, entrada.estadisticas);
            }
        }
    }

    public void RegistrarPuntos(int puntos)
    {
        if (puntos <= 0) return;
        puntosActuales += puntos;
        AlCambiarPuntos?.Invoke(puntosActuales);
    }

    public int ObtenerPrecio(CartaTorreta carta) => carta.Comprada ? carta.PrecioPagado
        : carta.Precio * SistemaPasivas.Eventos.MultiplicadorPrecio;

    public void RefrescarTienda() => AlCambiarCartas?.Invoke();

    public int CotizarVenta(TorreColocada torre) => SistemaPasivas.Eventos.VentaSinDevolucion ? 0
        : Mathf.FloorToInt(torre.PrecioPagado * porcentajeDevolucionVenta / 100f);

    public bool HayColocacionActiva => colocando;

    public bool IntentarVender(TorreColocada torre) => Vender(torre, false);

    public bool IntentarVenderDesdeInterfaz(TorreColocada torre) => Vender(torre, true);

    private bool Vender(TorreColocada torre, bool desdeInterfaz)
    {
        if (torre == null || torre.Colocador != this || !torre.isActiveAndEnabled
            || Time.timeScale == 0 || BloqueoEntradaUI.HayPanelAbierto
            || (!desdeInterfaz && BloqueoEntradaUI.BloqueaColocacion()) || !torre.MarcarVendida()) return false;
        int devolver = SistemaPasivas.Eventos.DevolucionVenta(torre.PrecioPagado, porcentajeDevolucionVenta, UnityEngine.Random.value);
        SistemaPasivas.RetirarTorre(torre);
        torre.gameObject.SetActive(false);
        Destroy(torre.gameObject);
        if (devolver > 0) RegistrarPuntos(devolver);
        return true;
    }

    public void ActualizarWave(int wave)
    {
        waveActual = Mathf.Max(1, wave);
    }

    void ReponerOfertas()
    {
        while (inventarioTorres.Count < MaximoCartasVisibles)
            if (!CrearOfertaAleatoria()) break;
    }

    bool CrearOfertaAleatoria()
    {
        if (tiposPorPrefab.Count == 0)
        {
            Debug.LogWarning("[TOWER] No hay torretas disponibles para vender.");
            return false;
        }

        List<GameObject> candidatas = ObtenerCandidatas();
        GameObject torre = SeleccionarTorrePonderada(candidatas);
        if (torre == null) return false;

        TowerType tipo = ObtenerTipoTorre(torre);
        tiposObtenidos.Add(tipo);
        tiposRecientes.Add(tipo);
        RarezaCarta rareza = torresObtenidas == 0 ? RarezaCarta.Normal
            : balanceTorretas.SortearRareza(waveActual, UnityEngine.Random.value);
        inventarioTorres.Add(new CartaTorreta(torre, tipo, rareza, balanceTorretas.bonosDanio.Multiplicador(rareza), preciosTorretas.Obtener(rareza)));
        torresEnInventario = inventarioTorres.Count;
        while (tiposRecientes.Count > Mathf.Max(0, recompensasAntiRepeticion))
        {
            tiposRecientes.RemoveAt(0);
        }

        torresObtenidas++;
        AlCambiarCartas?.Invoke();
        return true;
    }

    List<GameObject> ObtenerCandidatas()
    {
        List<TowerType> faltantes = new List<TowerType>();
        foreach (TowerPrefabEntry entrada in torresDisponibles)
        {
            if (entrada == null || entrada.prefab == null || ObtenerPesoTorre(entrada.prefab) <= 0) continue;
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
                if (entrada != null && entrada.prefab != null && ObtenerPesoTorre(entrada.prefab) > 0 && faltantes.Contains(entrada.tipo))
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
                if (entrada != null && entrada.prefab != null && ObtenerPesoTorre(entrada.prefab) > 0
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
            if (entrada != null && entrada.prefab != null && ObtenerPesoTorre(entrada.prefab) > 0)
            {
                disponibles.Add(entrada.prefab);
            }
        }
        return disponibles;
    }

    GameObject SeleccionarTorrePonderada(List<GameObject> candidatas)
    {
        var conPeso = candidatas.FindAll(t => t != null && ObtenerPesoTorre(t) > 0);
        var elegibles = conPeso.FindAll(t => !tiposRecientes.Contains(ObtenerTipoTorre(t)));
        if (elegibles.Count == 0) elegibles = conPeso;
        float total = 0f;
        foreach (var torre in elegibles) total += ObtenerPesoTorre(torre);
        if (total <= 0f) return null;
        float valor = UnityEngine.Random.value * total;
        foreach (var torre in elegibles)
        {
            valor -= ObtenerPesoTorre(torre);
            if (valor < 0f) return torre;
        }
        return elegibles[elegibles.Count - 1];
    }

    float ObtenerPesoTorre(GameObject torre)
    {
        return balanceTorretas.PesoTipo(ObtenerTipoTorre(torre), waveActual);
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
            bool entradaBloqueada = BloqueoEntradaUI.BloqueaColocacion();
            if (previewTorre != null)
                previewTorre.SetActive(!entradaBloqueada);
            if (entradaBloqueada)
                return;

            ActualizarPreview();
            if (previewTorre != null) previewTorre.SetActive(previewSobreTerreno);

            if (Input.GetMouseButtonDown(0))
            {
                IntentarColocarTorre();
            }

            // Escape lo gestiona PauseMenu; cancelar con click derecho evita que
            // abrir el menu destruya el preview de la torreta seleccionada.
            if (Input.GetMouseButtonDown(1))
            {
                CancelarColocacion();
            }
        }
    }

    // Compatibilidad con botones existentes: selecciona la primera carta de torreta.
    public void EmpezarColocacion()
    {
        if (inventarioTorres.Count > 0) SeleccionarCarta(inventarioTorres[0]);
    }

    public void SeleccionarCarta(CartaTorreta carta)
    {
        if (!isActiveAndEnabled || Time.timeScale == 0f || BloqueoEntradaUI.HayPanelAbierto
            || carta == null || !EsCartaVisible(carta)) return;
        bool yaComprada = carta.Comprada;
        if (!carta.Comprar(ref puntosActuales, ObtenerPrecio(carta))) return;
        if (!yaComprada) AlCambiarPuntos?.Invoke(puntosActuales);
        CancelarColocacion(false);
        CartaSeleccionada = carta;
        ultimoFrameSeleccion = Time.frameCount;
        torreSeleccionada = carta.Prefab;
        previewSobreTerreno = false;
        colocando = true;
        previewTorre = Instantiate(torreSeleccionada);
        previewTorre.SetActive(true);
        foreach (TowerAttackController ataque in previewTorre.GetComponentsInChildren<TowerAttackController>())
        {
            ataque.enabled = false;
        }
        TowerStats estadisticasPreview = previewTorre.GetComponent<TowerStats>();
        if (estadisticasPreview == null)
        {
            estadisticasPreview = previewTorre.AddComponent<TowerStats>();
        }
        estadisticasPreview.Configurar(ObtenerEstadisticasTorre(torreSeleccionada));
        ConfigurarTipoAutomatico(previewTorre, estadisticasPreview);
        PrepararVisualizadorRango(previewTorre);
        previewTorre.GetComponent<TowerRangeVisualizer>().SetVisible(true);
        // Desactivar collider del preview para que no interfiera con el raycast
        foreach (Collider col in previewTorre.GetComponentsInChildren<Collider>()) col.enabled = false;
        AplicarRareza(estadisticasPreview);
        estadisticasPreview.AgregarBonoColocacion(SistemaPasivas.Eventos.DanioPrimeraPendiente);
        previewTorre.SetActive(false);
        AlCambiarCartas?.Invoke();
    }

    private void ResolverCamaraDeColocacion()
    {
        // Usar la cámara aérea de esta escena aunque todavía no tenga la etiqueta MainCamera.
        foreach (ModoCamara modo in FindObjectsByType<ModoCamara>())
            if (modo.gameObject.scene == gameObject.scene && modo.camaraAerea != null)
            {
                camaraPrincipal = modo.camaraAerea;
                return;
            }
        camaraPrincipal = Camera.main;
    }

    void ActualizarPreview()
    {
        previewSobreTerreno = false;
        if (camaraPrincipal == null) ResolverCamaraDeColocacion();
        if (previewTorre == null || camaraPrincipal == null) return;

        Ray ray = camaraPrincipal.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            previewSobreTerreno = true;
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
        if (BloqueoEntradaUI.BloqueaColocacion() || ultimoFrameSeleccion == Time.frameCount
            || !previewSobreTerreno || CartaSeleccionada == null || !CartaSeleccionada.Comprada) return;

        TowerRangeVisualizer visualizador = previewTorre.GetComponent<TowerRangeVisualizer>();
        Vector3 centroTorre = visualizador.ObtenerCentroVisualWorld();
        if (!EsPosicionValida(centroTorre))
        {
            // No se puede colocar aca, no hacemos nada (el preview sigue activo)
            return;
        }

        // Confirmar colocacion: instanciar la torreta real
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
        estadisticas.Configurar(ObtenerEstadisticasTorre(torreSeleccionada));
        ConfigurarTipoAutomatico(torre, estadisticas);
        AplicarRareza(estadisticas);
        var colocada = torre.GetComponent<TorreColocada>();
        if (colocada == null) colocada = torre.AddComponent<TorreColocada>();
        colocada.Configurar(this, estadisticas, CartaSeleccionada.PrecioPagado);
        inventarioTorres.Remove(CartaSeleccionada);
        torresEnInventario = inventarioTorres.Count;

        CancelarColocacion();
        ReponerOfertas();
    }

    void AplicarRareza(TowerStats estadisticas)
    {
        if (CartaSeleccionada != null) estadisticas.EstablecerRareza(CartaSeleccionada.MultiplicadorDanio);
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

    TowerStatsData ObtenerEstadisticasTorre(GameObject torre)
    {
        if (torre != null
            && estadisticasPorPrefab.TryGetValue(torre, out TowerStatsData estadisticasPropias)
            && estadisticasPropias != null)
        {
            return estadisticasPropias;
        }

        return estadisticasTorre;
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

    private void OnDisable() => CancelarColocacion();

    public static bool CancelarColocacionActiva()
    {
        foreach (var colocador in FindObjectsByType<TowerPlacer>())
        {
            if (!colocador.colocando && colocador.ultimoFrameCancelacion != Time.frameCount) continue;
            if (colocador.colocando) colocador.CancelarColocacion();
            return true;
        }
        return false;
    }

    public void CancelarColocacion(bool notificar = true)
    {
        if (colocando && notificar) ultimoFrameCancelacion = Time.frameCount;
        if (previewTorre != null)
        {
            Destroy(previewTorre);
        }
        previewTorre = null;
        colocando = false;
        torreSeleccionada = null;
        CartaSeleccionada = null;
        previewSobreTerreno = false;
        if (notificar) AlCambiarCartas?.Invoke();
    }
}
