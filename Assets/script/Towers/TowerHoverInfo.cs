using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TowerStats))]
public class TowerHoverInfo : MonoBehaviour
{
    private TowerStats estadisticas;
    private TowerRangeVisualizer visualizadorRango;
    private GameObject panel;
    private GameObject canvasObject;
    private TextMeshProUGUI texto;
    private Camera camara;

    public void MostrarEstadisticas()
    {
        if (camara == null) camara = Camera.main;
        CrearPanel();
        ActualizarPanel();
    }

    public void OcultarEstadisticas()
    {
        if (panel != null) Destroy(panel);
        if (canvasObject != null) Destroy(canvasObject);
        panel = null;
        canvasObject = null;
        texto = null;
    }

    void Awake()
    {
        estadisticas = GetComponent<TowerStats>();
        visualizadorRango = GetComponent<TowerRangeVisualizer>();
        camara = Camera.main;
    }

    void OnMouseEnter()
    {
        if (camara == null) camara = Camera.main;
        if (visualizadorRango != null) visualizadorRango.SetVisible(true);
        CrearPanel();
        ActualizarPanel();
    }

    void LateUpdate()
    {
        if (panel == null) return;

        ActualizarPanel();
        Vector3 posicionTorre = camara.WorldToScreenPoint(transform.position);
        Vector3 posicionPantalla = posicionTorre;
        float desplazamientoX = posicionTorre.x < Screen.width * 0.65f ? 150f : -150f;
        float desplazamientoY = posicionTorre.y < Screen.height * 0.55f ? 92f : -92f;
        posicionPantalla += new Vector3(desplazamientoX, desplazamientoY, 0f);
        posicionPantalla.x = Mathf.Clamp(posicionPantalla.x, 120f, Screen.width - 120f);
        posicionPantalla.y = Mathf.Clamp(posicionPantalla.y, 95f, Screen.height - 95f);
        panel.transform.position = posicionPantalla;
    }

    void OnMouseExit()
    {
        if (visualizadorRango != null) visualizadorRango.SetVisible(false);
        OcultarEstadisticas();
    }

    void OnDestroy()
    {
        OcultarEstadisticas();
    }

    void CrearPanel()
    {
        if (panel != null) return;

        canvasObject = new GameObject("TowerInfoCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasObject.AddComponent<CanvasScaler>();

        panel = new GameObject("TowerInfoPanel");
        panel.transform.SetParent(canvas.transform, false);

        Image fondo = panel.AddComponent<Image>();
        fondo.color = new Color(0.035f, 0.055f, 0.07f, 0.9f);
        fondo.raycastTarget = false;

        Outline borde = panel.AddComponent<Outline>();
        borde.effectColor = new Color(0.55f, 0.82f, 0.68f, 0.75f);
        borde.effectDistance = new Vector2(2f, 2f);

        texto = new GameObject("Stats").AddComponent<TextMeshProUGUI>();
        texto.transform.SetParent(panel.transform, false);
        texto.font = TMP_Settings.defaultFontAsset;
        texto.fontSize = 13;
        texto.color = new Color(0.9f, 0.96f, 0.91f, 1f);
        texto.enabled = true;
        texto.alignment = TextAlignmentOptions.TopLeft;
        texto.raycastTarget = false;

        RectTransform rectTexto = texto.rectTransform;
        rectTexto.anchorMin = Vector2.zero;
        rectTexto.anchorMax = Vector2.one;
        rectTexto.offsetMin = new Vector2(12f, 9f);
        rectTexto.offsetMax = new Vector2(-12f, -9f);

        RectTransform rectPanel = panel.GetComponent<RectTransform>();
        rectPanel.sizeDelta = new Vector2(215f, 154f);
    }

    void ActualizarPanel()
    {
        if (texto == null || estadisticas == null) return;

        texto.text =
            $"{ObtenerNombreTipo()}\n" +
            $"Rango: {estadisticas.rango:0.0}\n" +
            $"Daño: {estadisticas.daño:0.0}\n" +
            $"Defensa: {estadisticas.defensa:0.0}\n" +
            $"Penetracion: {estadisticas.penetracion:0.0}\n" +
            $"Ataque cada: {estadisticas.tiempoEntreAtaques:0.00}s\n" +
            $"Objetivos: {estadisticas.cantidadObjetivos}";
    }

    string ObtenerNombreTipo()
    {
        switch (estadisticas.tipo)
        {
            case TowerType.Francotirador: return "FRANCOTIRADOR";
            case TowerType.Ametralladora: return "AMETRALLADORA";
            case TowerType.Canon: return "CANON";
            default: return "TORRE BASICA";
        }
    }
}
