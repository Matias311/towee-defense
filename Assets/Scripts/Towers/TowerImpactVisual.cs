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
    private TowerType tipo;
    private Vector3 origen;
    private LineRenderer trazoAtaque;

    public static void Mostrar(Vector3 posicion, float radio = 0f, float separacionVisual = 0.25f,
        TowerType tipoAtaque = TowerType.Basica, Vector3 origenAtaque = default)
    {
        GameObject objeto = new GameObject("ImpactoTorre");
        TowerImpactVisual efecto = objeto.AddComponent<TowerImpactVisual>();
        efecto.Inicializar(posicion, radio, separacionVisual, tipoAtaque, origenAtaque);
    }

    void Inicializar(Vector3 posicion, float radio, float separacionVisual, TowerType tipoAtaque, Vector3 origenAtaque)
    {
        centro = posicion;
        tipo = tipoAtaque;
        origen = origenAtaque;
        transform.position = posicion;
        radioArea = Mathf.Max(0f, radio);
        separacion = Mathf.Max(0.25f, separacionVisual);
        duracion = tipo == TowerType.Canon ? 0.62f
            : tipo == TowerType.Ametralladora ? 0.5f
            : tipo == TowerType.Francotirador ? 0.22f : 0.2f;
        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) { Destroy(gameObject); return; }
        material = new Material(shader);

        destello = new GameObject("Destello").transform;
        destello.SetParent(transform, false);
        CrearAnillo(destello, tipo == TowerType.Ametralladora ? 0.42f : 0.28f, false);
        int cantidadRayos = tipo == TowerType.Canon ? 12 : 8;
        for (int i = 0; i < cantidadRayos; i++)
        {
            float angulo = i * Mathf.PI * 2f / cantidadRayos;
            Vector3 direccion = new Vector3(Mathf.Cos(angulo), Mathf.Sin(angulo), 0f);
            float ancho = tipo == TowerType.Canon ? 0.1f : tipo == TowerType.Ametralladora ? 0.07f : 0.055f;
            LineRenderer linea = CrearLinea(destello, 2, ancho);
            linea.SetPosition(0, direccion * (tipo == TowerType.Canon ? 0.18f : 0.35f));
            linea.SetPosition(1, direccion * (i % 2 == 0 ? 0.8f : 0.6f));
        }
        CrearDetallePorTipo();
        if (radioArea > 0f)
        {
            onda = new GameObject("OndaArea").transform;
            onda.SetParent(transform, false);
            onda.localPosition = Vector3.up * 0.12f;
            CrearAnillo(onda, 1f, true);
        }
        lineas = GetComponentsInChildren<LineRenderer>();
        ActualizarVisual();
    }

    void CrearDetallePorTipo()
    {
        if (tipo == TowerType.Basica)
        {
            // Dos cortes cruzados hacen legible el golpe cuerpo a cuerpo rápido.
            CrearCorte(new Vector3(-0.45f, -0.38f, 0), new Vector3(0.45f, 0.38f, 0), 0.13f);
            CrearCorte(new Vector3(-0.38f, 0.4f, 0), new Vector3(0.38f, -0.4f, 0), 0.08f);
        }
        else if (tipo == TowerType.Ametralladora)
        {
            // Descargas dentadas alrededor del objetivo para acompañar la ralentización.
            for (int rayo = 0; rayo < 3; rayo++)
            {
                float angulo = rayo * Mathf.PI * 2f / 3f;
                Vector3 final = new Vector3(Mathf.Cos(angulo), Mathf.Sin(angulo), 0) * 0.7f;
                LineRenderer linea = CrearLinea(destello, 5, 0.075f);
                for (int punto = 0; punto < 5; punto++)
                {
                    float t = punto / 4f;
                    Vector3 posicion = Vector3.Lerp(Vector3.zero, final, t);
                    if (punto > 0 && punto < 4)
                        posicion += new Vector3(Mathf.Sin((punto + rayo) * 7f), Mathf.Cos((punto + rayo) * 5f), 0) * 0.12f;
                    linea.SetPosition(punto, posicion);
                }
            }
            CrearAnillo(destello, 0.62f, false);
        }
        else if (tipo == TowerType.Francotirador)
        {
            CrearAnillo(destello, 0.42f, false);
            CrearCorte(new Vector3(-0.56f, 0, 0), new Vector3(0.56f, 0, 0), 0.065f);
            CrearCorte(new Vector3(0, -0.56f, 0), new Vector3(0, 0.56f, 0), 0.065f);
            if ((centro - origen).sqrMagnitude > 0.1f)
            {
                trazoAtaque = CrearLinea(transform, 2, 0.055f);
                trazoAtaque.useWorldSpace = true;
                trazoAtaque.SetPosition(0, origen);
                trazoAtaque.SetPosition(1, centro);
            }
        }
        else
        {
            // Una segunda onda comunica peso y radio al impacto legendario.
            CrearAnillo(destello, 0.72f, false);
            CrearCorte(new Vector3(-0.65f, 0, 0), new Vector3(0.65f, 0, 0), 0.11f);
            CrearCorte(new Vector3(0, -0.65f, 0), new Vector3(0, 0.65f, 0), 0.11f);
        }
    }

    void CrearCorte(Vector3 desde, Vector3 hasta, float ancho)
    {
        LineRenderer linea = CrearLinea(destello, 2, ancho);
        linea.SetPosition(0, desde);
        linea.SetPosition(1, hasta);
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
        Color inicio = tipo == TowerType.Ametralladora ? new Color(0.25f, 0.95f, 1f)
            : tipo == TowerType.Francotirador ? new Color(1f, 0.98f, 0.75f)
            : tipo == TowerType.Canon ? new Color(1f, 0.82f, 0.18f)
            : new Color(1f, 0.95f, 0.78f);
        Color fin = tipo == TowerType.Ametralladora ? new Color(0.25f, 0.45f, 1f)
            : tipo == TowerType.Francotirador ? new Color(1f, 0.5f, 0.12f)
            : tipo == TowerType.Canon ? new Color(1f, 0.3f, 0.06f)
            : new Color(1f, 0.35f, 0.16f);
        Color color = Color.Lerp(inicio, fin, progreso);
        color.a = 1f - progreso * progreso;
        foreach (LineRenderer linea in lineas) { linea.startColor = color; linea.endColor = color; }
        if (trazoAtaque != null)
        {
            trazoAtaque.startColor = new Color(inicio.r, inicio.g, inicio.b, color.a);
            trazoAtaque.endColor = new Color(fin.r, fin.g, fin.b, color.a);
            trazoAtaque.widthMultiplier = Mathf.Lerp(0.08f, 0.015f, progreso);
        }
    }

    void OnDestroy()
    {
        if (material != null) Destroy(material);
    }
}
