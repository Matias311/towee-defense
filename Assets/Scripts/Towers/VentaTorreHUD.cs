using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Acción de venta para una torreta real seleccionada, sin pausar el juego.</summary>
public class VentaTorreHUD : MonoBehaviour
{
    private static VentaTorreHUD instancia;
    private TorreColocada torre;
    private RectTransform panel;
    private TMP_Text etiqueta;
    private Canvas canvas;
    private int frameSeleccion;

    public static bool EstaSeleccionada(TorreColocada candidata) =>
        instancia != null && candidata != null && instancia.torre == candidata;

    public static void Mostrar(TorreColocada seleccion)
    {
        if (instancia == null)
        {
            var objeto = new GameObject("VentaTorreHUD", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(GraphicRaycaster));
            instancia = objeto.AddComponent<VentaTorreHUD>();
            instancia.Crear();
        }
        instancia.torre = seleccion;
        seleccion.GetComponent<TowerHoverInfo>()?.OcultarEstadisticas();
        instancia.frameSeleccion = Time.frameCount;
        instancia.ActualizarVista();
    }

    private void Crear()
    {
        canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;
        var escala = GetComponent<CanvasScaler>();
        escala.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escala.referenceResolution = new Vector2(1920, 1080);
        escala.matchWidthOrHeight = .5f;
        var fondo = new GameObject("AccionesTorre", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        fondo.transform.SetParent(transform, false);
        fondo.color = new Color(.13f, .11f, .10f, .97f);
        panel = fondo.rectTransform;
        panel.sizeDelta = new Vector2(240, 105);
        Texto(panel, "Titulo", "Torreta seleccionada", new Vector2(0, 30), new Vector2(230, 32), 19);
        var imagen = new GameObject("Vender", typeof(RectTransform), typeof(Image), typeof(Button)).GetComponent<Image>();
        imagen.transform.SetParent(panel, false);
        imagen.color = new Color(.26f, .36f, .18f);
        imagen.rectTransform.anchoredPosition = new Vector2(0, -16);
        imagen.rectTransform.sizeDelta = new Vector2(220, 44);
        etiqueta = Texto(imagen.transform, "Importe", "Vender", Vector2.zero, new Vector2(210, 40), 21);
        Button boton = imagen.GetComponent<Button>();
        boton.targetGraphic = imagen;
        var navegacion = boton.navigation;
        navegacion.mode = Navigation.Mode.None;
        boton.navigation = navegacion;
        boton.onClick.AddListener(Vender);
    }

    private void LateUpdate()
    {
        if (torre == null || torre.Vendida || !torre.isActiveAndEnabled || torre.Colocador == null
            || !torre.Colocador.isActiveAndEnabled || torre.Colocador.HayColocacionActiva
            || Time.timeScale == 0 || BloqueoEntradaUI.HayPanelAbierto)
        {
            Cerrar();
            return;
        }
        ActualizarVista();
        if (Time.frameCount == frameSeleccion) return;
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)
            || (Input.GetMouseButtonDown(0)
                && !RectTransformUtility.RectangleContainsScreenPoint(panel, Input.mousePosition))) Cerrar();
    }

    private void ActualizarVista()
    {
        if (torre == null || torre.Colocador == null) return;
        etiqueta.text = "Vender · +" + torre.Colocador.CotizarVenta(torre) + " engranajes";
        Camera camara = torre.Colocador.camaraPrincipal;
        if (camara == null) camara = Camera.main;
        if (camara == null) return;
        Vector3 punto = camara.WorldToScreenPoint(torre.transform.position);
        if (punto.z <= 0) { panel.gameObject.SetActive(false); return; }
        panel.gameObject.SetActive(true);
        float mitadAncho = panel.rect.width * canvas.scaleFactor * .5f;
        float mitadAlto = panel.rect.height * canvas.scaleFactor * .5f;
        panel.position = new Vector3(
            Mathf.Clamp(punto.x, mitadAncho + 8, Screen.width - mitadAncho - 8),
            Mathf.Clamp(punto.y + 100 * canvas.scaleFactor, mitadAlto + 8, Screen.height - mitadAlto - 8), 0);
    }

    private void Vender()
    {
        if (torre != null && torre.Colocador != null
            && torre.Colocador.IntentarVenderDesdeInterfaz(torre)) Cerrar();
    }

    private void Cerrar()
    {
        if (instancia == this) instancia = null;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (instancia == this) instancia = null;
    }

    private static TMP_Text Texto(Transform padre, string nombre, string contenido, Vector2 posicion, Vector2 tamano, float fuente)
    {
        var texto = new GameObject(nombre, typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        texto.transform.SetParent(padre, false);
        texto.rectTransform.anchoredPosition = posicion;
        texto.rectTransform.sizeDelta = tamano;
        texto.font = TMP_Settings.defaultFontAsset;
        texto.fontSize = texto.fontSizeMax = fuente;
        texto.fontSizeMin = 14;
        texto.enableAutoSizing = true;
        texto.fontStyle = FontStyles.Bold;
        texto.color = new Color(1, .9f, .65f);
        texto.alignment = TextAlignmentOptions.Center;
        texto.raycastTarget = false;
        texto.text = contenido;
        return texto;
    }
}
