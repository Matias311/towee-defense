using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Cuatro ofertas fijas, con precio visible y una vista de detalle independiente.</summary>
[DisallowMultipleComponent]
public class CartasHUD : MonoBehaviour
{
    [SerializeField] private CartaFactory fabrica;
    [SerializeField] private TowerPlacer colocador;
    [Header("Barra de torretas")]
    [SerializeField, Min(0)] private float margenInferior = 24f;
    [SerializeField, Min(140)] private float anchoCarta = 170f;
    [SerializeField, Min(0)] private float separacion = 12f;
    [Tooltip("Opacidad de las ofertas cuando faltan engranajes para comprarlas.")]
    [SerializeField, Range(.1f, 1f)] private float opacidadNoDisponible = .35f;
    private const float AltoCarta = 300f;
    private RectTransform baraja;
    private Canvas canvas;
    private readonly List<OfertaVista> vistas = new List<OfertaVista>();
    private OfertaVista ofertaDetalle;
    private CartaUI detalle;

    private sealed class OfertaVista
    {
        public CartaTorreta Carta;
        public RectTransform Rect;
        public Button Boton;
        public Image FondoPrecio;
        public TMP_Text Precio;
        public CanvasGroup Grupo;
    }

    private void Awake()
    {
        foreach (Transform hijo in transform) hijo.gameObject.SetActive(false);
        baraja = new GameObject("BarraTorretas", typeof(RectTransform)).GetComponent<RectTransform>();
        baraja.SetParent(transform, false);
        baraja.anchorMin = baraja.anchorMax = new Vector2(1, 0);
        baraja.pivot = new Vector2(1, 0);
        baraja.anchoredPosition = new Vector2(-6, margenInferior);
        baraja.sizeDelta = new Vector2(anchoCarta * 4 + separacion * 3, AltoCarta);
        canvas = GetComponentInParent<Canvas>();
        RelojOleadasHUD.AsegurarConexion(canvas);
        if (colocador == null) colocador = FindAnyObjectByType<TowerPlacer>();
    }

    private void OnEnable()
    {
        if (colocador == null) colocador = FindAnyObjectByType<TowerPlacer>();
        if (colocador != null)
        {
            colocador.AlCambiarCartas += Actualizar;
            colocador.AlCambiarPuntos += ActualizarSaldo;
        }
        Actualizar();
    }

    private void OnDisable()
    {
        if (colocador != null)
        {
            colocador.AlCambiarCartas -= Actualizar;
            colocador.AlCambiarPuntos -= ActualizarSaldo;
        }
        OcultarDetalle();
    }

    private void LateUpdate()
    {
        Camera camara = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
            ? canvas.worldCamera : null;
        OfertaVista apuntada = null;
        if (!BloqueoEntradaUI.HayPanelAbierto && Time.timeScale > 0)
            foreach (OfertaVista vista in vistas)
                if (RectTransformUtility.RectangleContainsScreenPoint(vista.Rect, Input.mousePosition, camara))
                {
                    apuntada = vista;
                    break;
                }
        if (apuntada == ofertaDetalle) return;
        OcultarDetalle();
        if (apuntada != null) MostrarDetalle(apuntada);
    }

    private void Actualizar()
    {
        if (baraja == null || fabrica == null || colocador == null) return;
        OcultarDetalle();
        foreach (Transform hijo in baraja)
        {
            hijo.gameObject.SetActive(false);
            Destroy(hijo.gameObject);
        }
        vistas.Clear();
        int cantidad = Mathf.Min(4, colocador.CantidadCartasVisibles);
        for (int i = 0; i < cantidad; i++)
        {
            CartaTorreta carta = colocador.Cartas[i];
            Image fondo = Imagen("Oferta " + carta.Nombre, baraja);
            fondo.color = new Color(.13f, .11f, .10f, .95f);
            RectTransform rect = fondo.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(1, 0);
            rect.pivot = new Vector2(1, 0);
            rect.sizeDelta = new Vector2(anchoCarta, AltoCarta);
            rect.anchoredPosition = new Vector2(-(cantidad - 1 - i) * (anchoCarta + separacion), 0);
            Outline borde = fondo.gameObject.AddComponent<Outline>();
            borde.effectColor = colocador.CartaSeleccionada == carta
                ? new Color(1, .8f, .3f) : new Color(.5f, .42f, .3f);
            borde.effectDistance = new Vector2(2, -2);
            Texto(rect, "Nombre", carta.Nombre, .86f, .98f, 21);
            CartaUI arte = fabrica.Crear(carta.Rareza, "", "", rect);
            var arteRect = (RectTransform)arte.transform;
            arteRect.anchorMin = arteRect.anchorMax = new Vector2(.5f, 0);
            arteRect.pivot = new Vector2(.5f, 0);
            arteRect.anchoredPosition = new Vector2(0, 65);
            arteRect.localScale = Vector3.one * .38f;
            foreach (Graphic grafico in arte.GetComponentsInChildren<Graphic>()) grafico.raycastTarget = false;
            foreach (TMP_Text texto in arte.GetComponentsInChildren<TMP_Text>()) texto.gameObject.SetActive(false);
            Texto(rect, "Rareza", carta.Rareza.ToString(), .14f, .22f, 17);
            Image precio = Imagen("Precio", rect);
            precio.raycastTarget = false;
            Anclar(precio.rectTransform, .04f, .13f);
            TMP_Text cantidadTexto = Texto(precio.transform, "Cantidad", "", 0, 1, 21);
            CanvasGroup grupo = fondo.gameObject.AddComponent<CanvasGroup>();
            Button boton = fondo.gameObject.AddComponent<Button>();
            boton.targetGraphic = fondo;
            var navegacion = boton.navigation;
            navegacion.mode = Navigation.Mode.None;
            boton.navigation = navegacion;
            boton.onClick.AddListener(() => colocador.SeleccionarCarta(carta));
            vistas.Add(new OfertaVista { Carta = carta, Rect = rect, Boton = boton,
                FondoPrecio = precio, Precio = cantidadTexto, Grupo = grupo });
        }
        ActualizarSaldo(colocador.puntosActuales);
    }

    private void ActualizarSaldo(int saldo)
    {
        foreach (OfertaVista vista in vistas)
        {
            bool alcanza = vista.Carta.Comprada || saldo >= colocador.ObtenerPrecio(vista.Carta);
            vista.Boton.interactable = alcanza;
            vista.Grupo.alpha = alcanza ? 1f : opacidadNoDisponible;
            vista.FondoPrecio.color = alcanza ? new Color(.12f, .25f, .17f) : new Color(.36f, .13f, .12f);
            vista.Precio.text = vista.Carta.Comprada ? "Comprada · colocar"
                : colocador.ObtenerPrecio(vista.Carta) + " engranajes";
        }
        if (detalle != null && ofertaDetalle != null) ActualizarTextoDetalle();
    }

    private void MostrarDetalle(OfertaVista vista)
    {
        ofertaDetalle = vista;
        detalle = fabrica.Crear(vista.Carta.Rareza, vista.Carta.Nombre, "", baraja);
        var rect = (RectTransform)detalle.transform;
        rect.anchorMin = rect.anchorMax = new Vector2(1, 0);
        rect.pivot = new Vector2(1, 0);
        rect.localScale = Vector3.one * .8f;
        rect.anchoredPosition = new Vector2(vista.Rect.anchoredPosition.x, AltoCarta + 16);
        foreach (Graphic grafico in detalle.GetComponentsInChildren<Graphic>()) grafico.raycastTarget = false;
        ActualizarTextoDetalle();
    }

    private void ActualizarTextoDetalle()
    {
        CartaTorreta carta = ofertaDetalle.Carta;
        detalle.Mostrar(carta.Nombre, carta.Rareza + "\nDaño ×" + carta.MultiplicadorDanio.ToString("0.##")
            + "\n" + (carta.Comprada ? "Comprada · colocar" : colocador.ObtenerPrecio(carta) + " engranajes"));
        detalle.transform.Find("Estado").gameObject.SetActive(false);
    }

    private void OcultarDetalle()
    {
        if (detalle != null)
        {
            detalle.gameObject.SetActive(false);
            Destroy(detalle.gameObject);
        }
        detalle = null;
        ofertaDetalle = null;
    }

    private static Image Imagen(string nombre, Transform padre)
    {
        var imagen = new GameObject(nombre, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        imagen.transform.SetParent(padre, false);
        return imagen;
    }

    private static void Anclar(RectTransform rect, float abajo, float arriba)
    {
        rect.anchorMin = new Vector2(.05f, abajo);
        rect.anchorMax = new Vector2(.95f, arriba);
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

    private static TMP_Text Texto(Transform padre, string nombre, string contenido, float abajo, float arriba, float tamano)
    {
        var texto = new GameObject(nombre, typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        texto.transform.SetParent(padre, false);
        Anclar(texto.rectTransform, abajo, arriba);
        texto.font = TMP_Settings.defaultFontAsset;
        texto.fontSize = texto.fontSizeMax = tamano;
        texto.fontSizeMin = 13;
        texto.enableAutoSizing = true;
        texto.fontStyle = FontStyles.Bold;
        texto.alignment = TextAlignmentOptions.Center;
        texto.color = new Color(1, .9f, .65f);
        texto.raycastTarget = false;
        texto.richText = false;
        texto.text = contenido;
        return texto;
    }
}
