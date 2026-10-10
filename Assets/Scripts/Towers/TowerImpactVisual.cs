using UnityEngine;
using UnityEngine.Rendering;

// Vive separado del proyectil y del enemigo para seguir visible en impactos letales.
public class TowerImpactVisual : MonoBehaviour
{
    private Material material;
    private Transform destello;
    private Transform onda;
    private LineRenderer[] lineas;
    private float radioArea;
    private float tiempo;
    private float duracion;
    private Camera camara;
    private Vector3 centro;
    private float separacion;

    public static void Mostrar(Vector3 posicion, float radio = 0f, float separacionVisual = 0.25f)
    {
        GameObject objeto = new GameObject("ImpactoTorre");
        TowerImpactVisual efecto = objeto.AddComponent<TowerImpactVisual>();
        efecto.Inicializar(posicion, radio, separacionVisual);
    }

    void Inicializar(Vector3 posicion, float radio, float separacionVisual)
    {
        centro = posicion;
        transform.position = posicion;
        radioArea = Mathf.Max(0f, radio);
        separacion = Mathf.Max(0.25f, separacionVisual);
        duracion = radioArea > 0f ? 0.45f : 0.3f;
        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) { Destroy(gameObject); return; }
        material = new Material(shader);

        destello = new GameObject("Destello").transform;
        destello.SetParent(transform, false);
        CrearAnillo(destello, 0.28f, false);
        for (int i = 0; i < 8; i++)
        {
            float angulo = i * Mathf.PI / 4f;
            Vector3 direccion = new Vector3(Mathf.Cos(angulo), Mathf.Sin(angulo), 0f);
            LineRenderer linea = CrearLinea(destello, 2, 0.055f);
            linea.SetPosition(0, direccion * 0.35f);
            linea.SetPosition(1, direccion * (i % 2 == 0 ? 0.8f : 0.6f));
        }
        if (radioArea > 0f)
        {
            onda = new GameObject("OndaArea").transform;
            onda.SetParent(transform, false);
            CrearAnillo(onda, 1f, true);
        }
        lineas = GetComponentsInChildren<LineRenderer>();
        ActualizarVisual();
    }

    LineRenderer CrearLinea(Transform padre, int puntos, float ancho)
    {
        LineRenderer linea = new GameObject("Trazo").AddComponent<LineRenderer>();
        linea.transform.SetParent(padre, false);
        linea.sharedMaterial = material;
        linea.useWorldSpace = false;
        linea.positionCount = puntos;
        linea.widthMultiplier = ancho;
        linea.alignment = LineAlignment.View;
        linea.shadowCastingMode = ShadowCastingMode.Off;
        linea.receiveShadows = false;
        return linea;
    }

    void CrearAnillo(Transform padre, float radio, bool horizontal)
    {
        const int segmentos = 40;
        LineRenderer linea = CrearLinea(padre, segmentos, horizontal ? 0.08f : 0.045f);
        linea.loop = true;
        for (int i = 0; i < segmentos; i++)
        {
            float angulo = i * Mathf.PI * 2f / segmentos;
            float x = Mathf.Cos(angulo) * radio;
            float y = Mathf.Sin(angulo) * radio;
            linea.SetPosition(i, horizontal ? new Vector3(x, 0f, y) : new Vector3(x, y, 0f));
        }
    }

    void Update()
    {
        tiempo += Time.deltaTime;
        if (tiempo >= duracion) { Destroy(gameObject); return; }
        ActualizarVisual();
    }

    void LateUpdate()
    {
        if (destello == null) return;
        if (camara == null || !camara.isActiveAndEnabled) camara = Camera.main;
        if (camara == null) return;
        destello.rotation = camara.transform.rotation;
        // Sacar el destello del volumen del enemigo para que no quede oculto dentro.
        destello.position = centro + (camara.transform.position - centro).normalized * separacion;
    }

    void ActualizarVisual()
    {
        float progreso = Mathf.Clamp01(tiempo / duracion);
        destello.localScale = Vector3.one * Mathf.Lerp(0.45f, 1.15f, progreso);
        if (onda != null) onda.localScale = Vector3.one * Mathf.Lerp(0.15f, radioArea, progreso);
        Color color = Color.Lerp(new Color(1f, 0.95f, 0.65f), new Color(1f, 0.45f, 0.1f), progreso);
        color.a = 1f - progreso * progreso;
        foreach (LineRenderer linea in lineas) { linea.startColor = color; linea.endColor = color; }
    }

    void OnDestroy()
    {
        if (material != null) Destroy(material);
    }
}
