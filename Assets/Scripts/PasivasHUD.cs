using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Serialization;

/// <summary>Muestra las pasivas desbloqueadas al pulsar el icono del borde izquierdo.</summary>
[DisallowMultipleComponent]
public class PasivasHUD : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CartaFactory fabrica;
    [SerializeField] private RecompensaPasivasDatos configuracion;
    [FormerlySerializedAs("cartasAdicionales")]
    [Tooltip("Catálogo para desbloquear por índice. Estas cartas no se entregan al iniciar.")]
    [SerializeField, HideInInspector] private List<PasivaDatos> cartasDesbloqueables = new List<PasivaDatos>();

    private readonly List<CartaInstancia> mano = new List<CartaInstancia>();
    private bool inicializada;
    private readonly Queue<System.Action> recompensasPendientes = new Queue<System.Action>();
    private System.Action alElegir;
    private bool eligiendo;
    private float escalaTiempoAnterior;
    private List<PasivaDatos> opciones = new List<PasivaDatos>();

    public void OfrecerRecompensaBoss(System.Action alElegirRecompensa = null)
    {
        gameObject.SetActive(true);
        enabled = true;
        InicializarMano();
        CargarConfiguracion();
        recompensasPendientes.Enqueue(alElegirRecompensa);
        Debug.Log("[PASIVAS] Recompensa pendiente. Opciones en catálogo: " + cartasDesbloqueables.Count, this);
    }

    private void CargarConfiguracion()
    {
        if (configuracion == null) configuracion = Resources.Load<RecompensaPasivasDatos>("RecompensaPasivas");
        if (configuracion == null) return;
        if (fabrica == null) fabrica = configuracion.fabrica;
        if (icono == null) icono = configuracion.icono;
        cartasDesbloqueables = new List<PasivaDatos>(configuracion.catalogo);
    }

    [ContextMenu("Probar selección de pasiva (en Play)")]
    private void ProbarRecompensa()
    {
        if (Application.isPlaying) OfrecerRecompensaBoss();
    }

    private void Update()
    {
        SistemaPasivas.Actualizar(Time.deltaTime);
        if (recompensasPendientes.Count == 0 || Abierto || Time.timeScale == 0f || BloqueoEntradaUI.HayPanelAbierto) return;
        var adquiridas = new List<PasivaDatos>();
        foreach (var carta in mano) adquiridas.Add((PasivaDatos)carta.Datos);
        opciones = SorteoPasivas.Elegir(cartasDesbloqueables, adquiridas);
        alElegir = recompensasPendientes.Dequeue();
        if (opciones.Count == 0 || fabrica == null)
        {
            Debug.LogError("[PASIVAS] Falta la fábrica o el catálogo de pasivas. Se conserva la recompensa pendiente.", this);
            recompensasPendientes.Enqueue(alElegir);
            alElegir = null;
            enabled = false;
            return;
        }
        eligiendo = true;
        ActualizarAcceso();
        Abrir();
        if (!Abierto)
        {
            eligiendo = false;
            ActualizarAcceso();
            recompensasPendientes.Enqueue(alElegir);
            alElegir = null;
        }
    }
    [SerializeField] private Sprite icono;
    public int CantidadDesbloqueadas => mano.Count;

    private RectTransform pagina;
    private TMP_Text numeroPagina;
    private Button anterior;
    private Button siguiente;
    private int paginaActual;
    private const int cartasPorPagina = 4;
    private int Cantidad => mano.Count;
    private int Paginas => Mathf.Max(1, Mathf.CeilToInt(Cantidad / (float)cartasPorPagina));

    private GameObject ventana;
    private RectTransform contenido;
    private CursorLockMode bloqueoAnterior;
    private bool cursorVisibleAnterior;

    public bool Abierto => ventana != null && ventana.activeSelf;

    private void Awake()
    {
        SistemaPasivas.ReiniciarPartida();
        InicializarMano();
    }

    public void OnPointerClick(PointerEventData evento)
    {
        if (evento.button == PointerEventData.InputButton.Left && Cantidad > 0
            && !eligiendo && !BloqueoEntradaUI.HayPanelAbierto) Abrir();
    }

    public void Abrir()
    {
        if (Time.timeScale == 0f) return;
        if (Abierto || !isActiveAndEnabled)
            return;

        if (fabrica == null)
        {
            Debug.LogError("Asigna una CartaFactory a PanelCartas o usa Tools/Cartas/Configurar panel desplegable.", this);
            return;
        }

        escalaTiempoAnterior = Time.timeScale;
        if (eligiendo) Time.timeScale = 0f;
        bloqueoAnterior = Cursor.lockState;
        cursorVisibleAnterior = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ventana = new GameObject("PasivasDesplegadas", typeof(RectTransform), typeof(Canvas),
            typeof(GraphicRaycaster));
        BloqueoEntradaUI.EstablecerPanelAbierto(this, true);
        Canvas canvas = ventana.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var escalador = ventana.AddComponent<CanvasScaler>();
        escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escalador.referenceResolution = new Vector2(1920, 1080);
        escalador.matchWidthOrHeight = .5f;
        RectTransform raiz = (RectTransform)ventana.transform;
        Estirar(raiz);

        Image fondo = Imagen("FondoCerrar", raiz, null);
        Estirar(fondo.rectTransform);
        fondo.color = new Color(0, 0, 0, .38f);
        if (!eligiendo) fondo.gameObject.AddComponent<Button>().onClick.AddListener(Cerrar);

        contenido = new GameObject("Pasivas", typeof(RectTransform)).GetComponent<RectTransform>();
        contenido.SetParent(raiz, false);
        contenido.anchorMin = contenido.anchorMax = new Vector2(.5f, .5f);
        contenido.sizeDelta = new Vector2(1560, 740);
        contenido.anchoredPosition = Vector2.zero;

        pagina = new GameObject("Pagina", typeof(RectTransform)).GetComponent<RectTransform>();
        pagina.SetParent(contenido, false);
        Estirar(pagina);
        if (eligiendo)
        {
            var titulo = Etiqueta("ElegirPasiva", contenido);
            titulo.rectTransform.sizeDelta = new Vector2(1200, 60);
            titulo.rectTransform.anchoredPosition = new Vector2(0, 305);
            titulo.text = "Boss derrotado · Elige una pasiva";
            for (int i = 0; i < opciones.Count; i++)
            {
                PasivaDatos datos = opciones[i];
                CartaUI vista = fabrica.Crear(datos, pagina);
                var rect = (RectTransform)vista.transform;
                rect.anchoredPosition = new Vector2((i - (opciones.Count - 1) * .5f) * 390, 0);
                Button boton = vista.gameObject.AddComponent<Button>();
                boton.targetGraphic = vista.GetComponent<Image>();
                boton.onClick.AddListener(() => ElegirPasiva(datos));
                Boton("Elegir", contenido, new Vector2(rect.anchoredPosition.x, -290), () => ElegirPasiva(datos));
            }
            AjustarTamano();
            Debug.Log("[PASIVAS] Selección abierta con " + opciones.Count + " opciones.", this);
            return;
        }
        anterior = Boton("Anterior", contenido, new Vector2(-300, -350), () => MostrarPagina(paginaActual - 1));
        siguiente = Boton("Siguiente", contenido, new Vector2(300, -350), () => MostrarPagina(paginaActual + 1));
        numeroPagina = Etiqueta("NumeroPagina", contenido);
        numeroPagina.rectTransform.sizeDelta = new Vector2(600, 48);
        numeroPagina.rectTransform.anchoredPosition = new Vector2(0, 305);
        InicializarMano();
        MostrarPagina(0);

        Boton("Cerrar", contenido, new Vector2(0, -350), Cerrar);
        AjustarTamano();
    }

    /// <summary>Desbloquea una carta una sola vez durante esta partida.</summary>
    public bool Desbloquear(PasivaDatos datos)
    {
        if (datos == null) throw new System.ArgumentNullException(nameof(datos));
        InicializarMano();
        foreach (var carta in mano)
            if (carta.Datos == datos) return false;

        mano.Add(new CartaInstancia(datos));
        SistemaPasivas.Adquirir(datos);
        ActualizarAcceso();
        if (Abierto && !eligiendo) MostrarPagina(Paginas - 1);
        return true;
    }

    // Compatible con llamadas existentes; agregar ahora significa desbloquear.
    public void Agregar(PasivaDatos datos) => Desbloquear(datos);

    /// <summary>Conectar desde un UnityEvent usando el índice del catálogo (desde cero).</summary>
    public void DesbloquearPorIndice(int indice)
    {
        if (indice < 0 || indice >= cartasDesbloqueables.Count || cartasDesbloqueables[indice] == null)
        {
            Debug.LogWarning("Índice de carta inválido: " + indice, this);
            return;
        }
        Desbloquear(cartasDesbloqueables[indice]);
    }

    private void InicializarMano()
    {
        if (inicializada) return;
        inicializada = true;
        CargarConfiguracion();
        // El controlador de respaldo creado por el boss también necesita un Canvas para el icono.
        if (GetComponentInParent<Canvas>() == null)
        {
            Canvas destino = null;
            foreach (var canvas in FindObjectsByType<Canvas>())
                if (canvas.isRootCanvas && canvas.name == "HUD") { destino = canvas; break; }
            if (destino == null)
            {
                var raiz = new GameObject("AccesoPasivas", typeof(RectTransform), typeof(Canvas),
                    typeof(CanvasScaler), typeof(GraphicRaycaster));
                destino = raiz.GetComponent<Canvas>();
                destino.renderMode = RenderMode.ScreenSpaceOverlay;
                var escalador = raiz.GetComponent<CanvasScaler>();
                escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                escalador.referenceResolution = new Vector2(1920, 1080);
            }
            transform.SetParent(destino.transform, false);
        }
        var rect = (RectTransform)transform;
        rect.anchorMin = rect.anchorMax = new Vector2(0, .5f);
        rect.pivot = new Vector2(0, .5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(169, 227);
        Image imagen = GetComponent<Image>();
        if (imagen == null) imagen = gameObject.AddComponent<Image>();
        imagen.sprite = icono;
        imagen.preserveAspect = true;
        ActualizarAcceso();
    }

    private void ActualizarAcceso()
    {
        Image imagen = GetComponent<Image>();
        if (imagen == null) return;
        imagen.sprite = icono;
        bool visible = Cantidad > 0 && !eligiendo;
        imagen.enabled = visible;
        imagen.raycastTarget = visible;
    }

    private void MostrarPagina(int indicePagina)
    {
        paginaActual = Mathf.Clamp(indicePagina, 0, Paginas - 1);
        foreach (Transform hijo in pagina)
        {
            hijo.gameObject.SetActive(false);
            Destroy(hijo.gameObject);
        }
        int inicio = paginaActual * cartasPorPagina;
        int cantidadVisible = Mathf.Min(cartasPorPagina, Cantidad - inicio);
        for (int i = 0; i < cantidadVisible; i++)
        {
            int indice = inicio + i;
            CartaInstancia instancia = mano[indice];
            CartaUI vista = fabrica.Crear(instancia.Datos, pagina);
            vista.Vincular(instancia);
            RectTransform rect = (RectTransform)vista.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
            rect.anchoredPosition = new Vector2((i - (cantidadVisible - 1) * .5f) * 390, 0);
        }
        numeroPagina.text = Cantidad == 0 ? "Pasivas · 0" : "Pasivas · " + (paginaActual + 1) + " / " + Paginas;
        if (Cantidad == 0)
        {
            TMP_Text vacio = Etiqueta("SinCartas", pagina);
            vacio.rectTransform.sizeDelta = new Vector2(1000, 160);
            vacio.text = "Aún no tienes pasivas.\nSe desbloquearán con tu progreso.";
        }
        anterior.interactable = paginaActual > 0;
        siguiente.interactable = paginaActual < Paginas - 1;
    }

    private static Button Boton(string nombre, Transform padre, Vector2 posicion, UnityEngine.Events.UnityAction accion)
    {
        Image imagen = Imagen(nombre, padre, null);
        imagen.color = new Color(.18f, .14f, .12f);
        imagen.rectTransform.anchorMin = imagen.rectTransform.anchorMax = new Vector2(.5f, .5f);
        imagen.rectTransform.sizeDelta = new Vector2(180, 52);
        imagen.rectTransform.anchoredPosition = posicion;
        Button boton = imagen.gameObject.AddComponent<Button>();
        boton.onClick.AddListener(accion);
        TMP_Text texto = Etiqueta("Texto", imagen.transform);
        Estirar(texto.rectTransform);
        texto.text = nombre;
        return boton;
    }

    private static TMP_Text Etiqueta(string nombre, Transform padre)
    {
        var texto = new GameObject(nombre, typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        texto.transform.SetParent(padre, false);
        texto.font = TMP_Settings.defaultFontAsset;
        texto.fontSize = 26;
        texto.alignment = TextAlignmentOptions.Center;
        texto.raycastTarget = false;
        return texto;
    }

    private void LateUpdate()
    {
        if (Abierto)
            AjustarTamano();
    }

    private void AjustarTamano()
    {
        Rect rect = ((RectTransform)ventana.transform).rect;
        float escala = Mathf.Max(.01f, Mathf.Min(1f, (rect.width - 60) / 1560f, (rect.height - 60) / 780f));
        contenido.localScale = Vector3.one * escala;
    }

    public static bool CerrarVentanaAbierta()
    {
        foreach (var hud in FindObjectsByType<PasivasHUD>())
        {
            if (!hud.Abierto) continue;
            if (!hud.eligiendo) hud.Cerrar();
            return true;
        }
        return false;
    }

    private void ElegirPasiva(PasivaDatos datos)
    {
        if (!eligiendo || !opciones.Contains(datos)) return;
        // Cada boss entrega una copia, incluso cuando ya se posee esa definición.
        mano.Add(new CartaInstancia(datos));
        SistemaPasivas.Adquirir(datos);
        eligiendo = false;
        ActualizarAcceso();
        CerrarVentana();
        var continuar = alElegir;
        alElegir = null;
        continuar?.Invoke();
    }

    public void Cerrar()
    {
        if (eligiendo) return;
        CerrarVentana();
    }

    private void CerrarVentana()
    {
        BloqueoEntradaUI.EstablecerPanelAbierto(this, false);
        if (ventana == null)
            return;
        ventana.SetActive(false);
        Destroy(ventana);
        ventana = null;
        Time.timeScale = escalaTiempoAnterior;
        Cursor.lockState = bloqueoAnterior;
        Cursor.visible = cursorVisibleAnterior;
    }

    private void OnDisable()
    {
        eligiendo = false;
        CerrarVentana();
    }

    private static Image Imagen(string nombre, Transform padre, Sprite sprite)
    {
        var imagen = new GameObject(nombre, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        imagen.transform.SetParent(padre, false);
        imagen.sprite = sprite;
        imagen.preserveAspect = sprite != null;
        return imagen;
    }

    private static void Estirar(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }
}
