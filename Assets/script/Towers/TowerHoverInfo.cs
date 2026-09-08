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
        PrepararColliderHover();
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

    void PrepararColliderHover()
    {
        if (GetComponent<Collider>() != null) return;

        Renderer[] renderizadores = GetComponentsInChildren<Renderer>(true);
        Bounds limites = new Bounds(transform.position, Vector3.zero);
        bool hayRenderizador = false;

        foreach (Renderer renderizador in renderizadores)
        {
            if (renderizador.transform == transform
                || renderizador.name == "Rango"
                || renderizador.name == "HaloRango"
                || renderizador.name == "AreaRango")
            {
                continue;
            }

            if (!hayRenderizador)
            {
                limites = renderizador.bounds;
                hayRenderizador = true;
            }
            else
            {
                limites.Encapsulate(renderizador.bounds);
            }
        }

        if (!hayRenderizador) return;

        BoxCollider colliderHover = gameObject.AddComponent<BoxCollider>();
        colliderHover.center = transform.InverseTransformPoint(limites.center);
        Vector3 tamaño = transform.InverseTransformVector(limites.size);
        colliderHover.size = new Vector3(
            Mathf.Abs(tamaño.x),
            Mathf.Abs(tamaño.y),
            Mathf.Abs(tamaño.z)
        );
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
        rectPanel.sizeDelta = new Vector2(235f, 190f);
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
            $"Tipo de ataque: {estadisticas.tipoAtaque}\n" +
            $"Objetivos: {estadisticas.cantidadObjetivos}\n" +
            $"Prioridad: {estadisticas.prioridadObjetivo}\n" +
            $"Proyectil: {(estadisticas.usarProyectil ? "SI" : "NO")}";
    }

    string ObtenerNombreTipo()
    {
        return estadisticas.tipo.ToString().ToUpperInvariant();
    }
}
