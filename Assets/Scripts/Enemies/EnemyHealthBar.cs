using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image rellenoVida;
    [SerializeField] private Canvas canvasVida;
    [SerializeField] private Camera camara;
    [SerializeField] private bool ocultarConVidaCompleta = false;
    [SerializeField] private Vector2 tamaño = new Vector2(110f, 16f);
    [SerializeField] private Sprite marcoVida;

    private EnemyStats estadisticas;

    void Awake()
    {
        estadisticas = GetComponentInParent<EnemyStats>();

        if (canvasVida == null)
        {
            canvasVida = GetComponent<Canvas>();
        }

        if (canvasVida == null)
        {
            CrearBarra();
        }
    }

    void Start()
    {
        ActualizarCamara();
        ActualizarBarra();
    }

    void LateUpdate()
    {
        if (estadisticas == null)
        {
            estadisticas = GetComponentInParent<EnemyStats>();
        }

        if (camara == null)
        {
            ActualizarCamara();
        }

        ActualizarBarra();
        GirarHaciaLaCamara();
    }

    void ActualizarCamara()
    {
        if (camara == null)
        {
            camara = Camera.main;
        }
    }

    void ActualizarBarra()
    {
        if (estadisticas == null || rellenoVida == null) return;

        float porcentaje = estadisticas.vidaMaxima > 0f
            ? Mathf.Clamp01(estadisticas.vidaActual / estadisticas.vidaMaxima)
            : 0f;

        rellenoVida.fillAmount = porcentaje;

        if (canvasVida != null && ocultarConVidaCompleta)
        {
            canvasVida.enabled = porcentaje < 1f;
        }
    }

    void CrearBarra()
    {
        GameObject objetoCanvas = new GameObject("EnemyHealthBarCanvas");
        objetoCanvas.transform.SetParent(transform, false);

        canvasVida = objetoCanvas.AddComponent<Canvas>();
        canvasVida.renderMode = RenderMode.WorldSpace;
        canvasVida.overrideSorting = true;
        canvasVida.sortingOrder = 10;

        float escalaPadre = Mathf.Max(
            Mathf.Abs(transform.lossyScale.x),
            Mathf.Abs(transform.lossyScale.y),
            Mathf.Abs(transform.lossyScale.z)
        );
        escalaPadre = Mathf.Max(escalaPadre, 0.001f);
        objetoCanvas.transform.localScale = Vector3.one * (0.022f / escalaPadre);
        objetoCanvas.transform.localPosition = Vector3.up * (1.8f / escalaPadre);

        GameObject fondo = CrearImagen("HealthBarBackground", objetoCanvas.transform);
        RectTransform rectFondo = fondo.GetComponent<RectTransform>();
        rectFondo.sizeDelta = tamaño;
        Image imagenFondo = fondo.GetComponent<Image>();
        imagenFondo.sprite = marcoVida;
        imagenFondo.color = marcoVida != null
            ? Color.white
            : new Color(0.03f, 0.03f, 0.03f, 1f);

        GameObject relleno = CrearImagen("HealthBarFill", fondo.transform);
        RectTransform rectRelleno = relleno.GetComponent<RectTransform>();
        rectRelleno.anchorMin = Vector2.zero;
        rectRelleno.anchorMax = Vector2.one;
        rectRelleno.offsetMin = new Vector2(4f, 4f);
        rectRelleno.offsetMax = new Vector2(-4f, -4f);

        rellenoVida = relleno.GetComponent<Image>();
        rellenoVida.sprite = Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f),
            1f
        );
        rellenoVida.type = Image.Type.Filled;
        rellenoVida.fillMethod = Image.FillMethod.Horizontal;
        rellenoVida.fillOrigin = (int)Image.OriginHorizontal.Left;
        rellenoVida.color = new Color(0.9f, 0.08f, 0.08f, 1f);
    }

    public void ConfigurarMarco(Sprite sprite)
    {
        marcoVida = sprite;

        if (canvasVida == null) return;

        Transform fondo = canvasVida.transform.Find("HealthBarBackground");
        Image imagenFondo = fondo != null ? fondo.GetComponent<Image>() : null;
        if (imagenFondo == null) return;

        imagenFondo.sprite = marcoVida;
        imagenFondo.color = marcoVida != null
            ? Color.white
            : new Color(0.03f, 0.03f, 0.03f, 1f);
    }

    GameObject CrearImagen(string nombre, Transform padre)
    {
        GameObject imagen = new GameObject(nombre, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imagen.transform.SetParent(padre, false);

        RectTransform rect = imagen.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = tamaño;
        return imagen;
    }

    void GirarHaciaLaCamara()
    {
        if (canvasVida == null || camara == null) return;

        canvasVida.worldCamera = camara;
        Vector3 direccion = canvasVida.transform.position - camara.transform.position;
        if (direccion.sqrMagnitude > 0.0001f)
        {
            canvasVida.transform.rotation = Quaternion.LookRotation(direccion);
        }
    }
}
