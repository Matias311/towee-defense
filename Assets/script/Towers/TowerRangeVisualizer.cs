using UnityEngine;

[RequireComponent(typeof(TowerStats))]
public class TowerRangeVisualizer : MonoBehaviour {
    [SerializeField] private Color colorRango = new Color(0.55f, 0.9f, 0.68f, 0.72f);
    [SerializeField] private Color colorHalo = new Color(0.55f, 0.9f, 0.68f, 0.08f);
    [SerializeField] private Color colorArea = new Color(0.55f, 0.9f, 0.68f, 0.012f);
    [SerializeField] private int segmentos = 64;

    private TowerStats estadisticas;
    private LineRenderer linea;
    private LineRenderer halo;
    private MeshRenderer area;
    private Vector3 centroVisualLocal;

    void Awake() {
        estadisticas = GetComponent<TowerStats>();
        centroVisualLocal = CalcularCentroVisualLocal();
        CrearLinea();
        CrearArea();
        SetVisible(false);
    }

    void LateUpdate() {
        if (linea == null || !linea.enabled) return;
        linea.startColor = colorRango;
        linea.endColor = colorRango;
        linea.widthMultiplier = 0.022f;
        halo.startColor = colorHalo;
        halo.endColor = colorHalo;
        halo.widthMultiplier = 0.07f;
        ActualizarLinea();
        if (area != null) {
            area.transform.localScale = new Vector3(
                estadisticas.rango * 2f,
                0.01f,
                estadisticas.rango * 2f
            );
        }
    }

    void CrearLinea() {
        GameObject objetoLinea = new GameObject("Rango");
        objetoLinea.transform.SetParent(transform, false);
        linea = objetoLinea.AddComponent<LineRenderer>();
        linea.useWorldSpace = false;
        linea.loop = true;
        linea.positionCount = Mathf.Max(16, segmentos);
        linea.startWidth = 0.022f;
        linea.endWidth = 0.022f;
        linea.alignment = LineAlignment.View;
        linea.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        linea.receiveShadows = false;
        linea.startColor = colorRango;
        linea.endColor = colorRango;
        linea.material = CrearMaterial(colorRango);

        GameObject objetoHalo = new GameObject("HaloRango");
        objetoHalo.transform.SetParent(transform, false);
        halo = objetoHalo.AddComponent<LineRenderer>();
        halo.useWorldSpace = false;
        halo.loop = true;
        halo.positionCount = Mathf.Max(16, segmentos);
        halo.startWidth = 0.07f;
        halo.endWidth = 0.07f;
        halo.alignment = LineAlignment.View;
        halo.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        halo.receiveShadows = false;
        halo.startColor = colorHalo;
        halo.endColor = colorHalo;
        halo.material = CrearMaterial(colorHalo);
        ActualizarLinea();
    }

    public void SetVisible(bool visible)
    {
        if (linea != null) linea.enabled = visible;
        if (halo != null) halo.enabled = visible;
        if (area != null) area.enabled = visible;
    }

    void CrearArea() {
        GameObject objetoArea = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        objetoArea.name = "AreaRango";
        objetoArea.transform.SetParent(transform, false);
        objetoArea.transform.localPosition = centroVisualLocal + new Vector3(0f, 0.015f, 0f);
        objetoArea.transform.localScale = new Vector3(1f, 0.01f, 1f);

        Collider colision = objetoArea.GetComponent<Collider>();
        if (colision != null) Destroy(colision);

        area = objetoArea.GetComponent<MeshRenderer>();
        area.material = CrearMaterial(colorArea);
    }

    Vector3 CalcularCentroVisualLocal() {
        Renderer[] renderizadores = GetComponentsInChildren<Renderer>(true);
        if (renderizadores.Length == 0) return Vector3.zero;

        Bounds limites = renderizadores[0].bounds;
        for (int i = 1; i < renderizadores.Length; i++) {
            limites.Encapsulate(renderizadores[i].bounds);
        }

        Vector3 centro = transform.InverseTransformPoint(limites.center);
        centro.y = 0f;
        return centro;
    }

    public void ColocarCentroVisualEn(Vector3 posicion) {
        transform.position = posicion - transform.TransformVector(centroVisualLocal);
    }

    public Vector3 ObtenerCentroVisualWorld() {
        return transform.TransformPoint(centroVisualLocal);
    }

    void ActualizarLinea() {
        if (linea == null || estadisticas == null) return;

        int cantidadSegmentos = Mathf.Max(16, segmentos);
        linea.positionCount = cantidadSegmentos;
        if (halo != null) halo.positionCount = cantidadSegmentos;
        float radio = Mathf.Max(0.01f, estadisticas.rango);

        for (int i = 0; i < cantidadSegmentos; i++) {
            float angulo = i * Mathf.PI * 2f / cantidadSegmentos;
            Vector3 posicion = new Vector3(
                centroVisualLocal.x + Mathf.Cos(angulo) * radio,
                centroVisualLocal.y + 0.03f,
                centroVisualLocal.z + Mathf.Sin(angulo) * radio
            );
            linea.SetPosition(i, posicion);
            if (halo != null) halo.SetPosition(i, posicion);
        }
    }

    Material CrearMaterial() {
        return CrearMaterial(colorRango);
    }

    Material CrearMaterial(Color color) {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) shader = Shader.Find("Unlit/Color");
        if (shader == null) shader = Shader.Find("Sprites/Default");

        Material material = new Material(shader);
        material.color = color;
        if (material.HasProperty("_Surface")) {
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Blend", 0f);
            material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetFloat("_ZWrite", 0f);
            material.renderQueue = 3000;
        }
        return material;
    }
}
