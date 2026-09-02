using UnityEngine;

[RequireComponent(typeof(TowerStats))]
public class TowerRangeVisualizer : MonoBehaviour {
    [SerializeField] private Color colorRango = new Color(0f, 1f, 0f, 1f);
    [SerializeField] private Color colorArea = new Color(0f, 1f, 0f, 0.08f);
    [SerializeField] private int segmentos = 64;

    private TowerStats estadisticas;
    private LineRenderer linea;
    private MeshRenderer area;

    void Awake() {
        estadisticas = GetComponent<TowerStats>();
        CrearLinea();
        CrearArea();
    }

    void LateUpdate() {
        if (linea == null) return;
        linea.startColor = colorRango;
        linea.endColor = colorRango;
        linea.widthMultiplier = 0.1f;
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
        linea.startWidth = 0.1f;
        linea.endWidth = 0.1f;
        linea.alignment = LineAlignment.View;
        linea.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        linea.receiveShadows = false;
        linea.startColor = colorRango;
        linea.endColor = colorRango;
        linea.material = CrearMaterial();
        ActualizarLinea();
    }

    void CrearArea() {
        GameObject objetoArea = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        objetoArea.name = "AreaRango";
        objetoArea.transform.SetParent(transform, false);
        objetoArea.transform.localPosition = new Vector3(0f, 0.015f, 0f);
        objetoArea.transform.localScale = new Vector3(1f, 0.01f, 1f);

        Collider colision = objetoArea.GetComponent<Collider>();
        if (colision != null) Destroy(colision);

        area = objetoArea.GetComponent<MeshRenderer>();
        area.material = CrearMaterial(colorArea);
    }

    void ActualizarLinea() {
        if (linea == null || estadisticas == null) return;

        int cantidadSegmentos = Mathf.Max(16, segmentos);
        linea.positionCount = cantidadSegmentos;
        float radio = Mathf.Max(0.01f, estadisticas.rango);

        for (int i = 0; i < cantidadSegmentos; i++) {
            float angulo = i * Mathf.PI * 2f / cantidadSegmentos;
            linea.SetPosition(i, new Vector3(
                Mathf.Cos(angulo) * radio,
                0.03f,
                Mathf.Sin(angulo) * radio
            ));
        }
    }

    Material CrearMaterial() {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) shader = Shader.Find("Unlit/Color");
        if (shader == null) shader = Shader.Find("Sprites/Default");

        Material material = new Material(shader);
        material.color = colorRango;
        return material;
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
