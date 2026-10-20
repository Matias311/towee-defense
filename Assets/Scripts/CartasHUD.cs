using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Abre una vista de lectura de las cartas al pulsar el abanico.</summary>
[DisallowMultipleComponent]
public class CartasHUD : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CartaFactory fabrica;
    [Tooltip("Cartas nuevas que se suman a las cuatro cartas originales del HUD.")]
    [SerializeField] private List<CartaDatos> cartasAdicionales = new List<CartaDatos>();

    private readonly List<CartaUI> originales = new List<CartaUI>();
    private readonly List<CartaDatos> adicionalesActivas = new List<CartaDatos>();
    private RectTransform pagina;
    private TMP_Text numeroPagina;
    private Button anterior;
    private Button siguiente;
    private int paginaActual;
    private const int cartasPorPagina = 4;
    private int Cantidad => originales.Count + adicionalesActivas.Count;
    private int Paginas => Mathf.Max(1, Mathf.CeilToInt(Cantidad / (float)cartasPorPagina));

    private GameObject ventana;
    private RectTransform contenido;
    private CursorLockMode bloqueoAnterior;
    private bool cursorVisibleAnterior;

    public bool Abierto => ventana != null && ventana.activeSelf;

    private void Start()
    {
        foreach (Image imagen in GetComponentsInChildren<Image>(true))
            imagen.raycastTarget = true;

        // Los detalles se leen en la vista desplegada, no en el abanico.
        foreach (TMP_Text texto in GetComponentsInChildren<TMP_Text>(true))
            texto.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData evento)
    {
        if (evento.button == PointerEventData.InputButton.Left)
            Abrir();
    }

    public void Abrir()
    {
        if (Abierto || !isActiveAndEnabled)
            return;

        if (fabrica == null)
        {
            Debug.LogError("Asigna una CartaFactory a PanelCartas o usa Tools/Cartas/Configurar panel desplegable.", this);
            return;
        }

        bloqueoAnterior = Cursor.lockState;
        cursorVisibleAnterior = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ventana = new GameObject("CartasDesplegadas", typeof(RectTransform), typeof(Canvas),
            typeof(GraphicRaycaster));
        BloqueoEntradaUI.EstablecerPanelAbierto(this, true);
        ventana.transform.SetParent(GetComponentInParent<Canvas>().rootCanvas.transform, false);
        Canvas canvas = ventana.GetComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 50;
        RectTransform raiz = (RectTransform)ventana.transform;
        Estirar(raiz);

        Image fondo = Imagen("FondoCerrar", raiz, null);
        Estirar(fondo.rectTransform);
        fondo.color = new Color(0, 0, 0, .38f);
        fondo.gameObject.AddComponent<Button>().onClick.AddListener(Cerrar);

        contenido = new GameObject("Cartas", typeof(RectTransform)).GetComponent<RectTransform>();
        contenido.SetParent(raiz, false);
        contenido.anchorMin = contenido.anchorMax = new Vector2(.5f, .5f);
        contenido.sizeDelta = new Vector2(1560, 620);
        contenido.anchoredPosition = Vector2.zero;

        pagina = new GameObject("Pagina", typeof(RectTransform)).GetComponent<RectTransform>();
        pagina.SetParent(contenido, false);
        Estirar(pagina);
        anterior = Boton("Anterior", contenido, new Vector2(-300, -305), () => MostrarPagina(paginaActual - 1));
        siguiente = Boton("Siguiente", contenido, new Vector2(300, -305), () => MostrarPagina(paginaActual + 1));
        numeroPagina = Etiqueta("NumeroPagina", contenido);
        numeroPagina.rectTransform.sizeDelta = new Vector2(600, 48);
        numeroPagina.rectTransform.anchoredPosition = new Vector2(0, 305);
        ActualizarCartas();
        MostrarPagina(0);

        Boton("Cerrar", contenido, new Vector2(0, -305), Cerrar);
        AjustarTamano();
    }

    /// <summary>Agrega una definicion a esta mano, sin modificar el asset compartido.</summary>
    public void Agregar(CartaDatos datos)
    {
        if (datos == null) throw new System.ArgumentNullException(nameof(datos));
        cartasAdicionales.Add(datos);
        if (Abierto)
        {
            ActualizarCartas();
            MostrarPagina(Paginas - 1);
        }
    }

    private void ActualizarCartas()
    {
        originales.Clear();
        adicionalesActivas.Clear();
        foreach (Transform hijo in transform)
            if (hijo.TryGetComponent(out CartaUI carta))
                originales.Add(carta);
        foreach (CartaDatos datos in cartasAdicionales)
            if (datos != null)
                adicionalesActivas.Add(datos);
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
            CartaUI vista;
            if (indice < originales.Count)
            {
                CartaUI original = originales[indice];
                vista = fabrica.Crear(original.Rareza, original.Nombre, original.Efecto, pagina);
                vista.CambiarEstado(original.Estado);
            }
            else
            {
                vista = fabrica.Crear(adicionalesActivas[indice - originales.Count], pagina);
            }
            RectTransform rect = (RectTransform)vista.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
            rect.anchoredPosition = new Vector2((i - (cantidadVisible - 1) * .5f) * 390, 0);
        }
        numeroPagina.text = "Cartas · " + (paginaActual + 1) + " / " + Paginas;
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
        float escala = Mathf.Max(.01f, Mathf.Min(1f, (rect.width - 60) / 1560f, (rect.height - 60) / 680f));
        contenido.localScale = Vector3.one * escala;
    }

    public void Cerrar()
    {
        BloqueoEntradaUI.EstablecerPanelAbierto(this, false);
        if (ventana == null)
            return;
        ventana.SetActive(false);
        Destroy(ventana);
        ventana = null;
        Cursor.lockState = bloqueoAnterior;
        Cursor.visible = cursorVisibleAnterior;
    }

    private void OnDisable() => Cerrar();

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
