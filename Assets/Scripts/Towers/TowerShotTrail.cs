using UnityEngine;
using UnityEngine.Rendering;

// La estela vive separada para desvanecerse después de que la bala impacte.
public class TowerShotTrail : MonoBehaviour
{
    private Transform proyectil;
    private TrailRenderer estela;
    private Material material;
    private bool finalizada;

    public static void Crear(Transform origen, Color color, float ancho, float duracion)
    {
        GameObject objeto = new GameObject("EstelaDisparo");
        objeto.transform.position = origen.position;
        TowerShotTrail efecto = objeto.AddComponent<TowerShotTrail>();
        efecto.proyectil = origen;
        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) { Destroy(objeto); return; }
        efecto.material = new Material(shader);
        efecto.estela = objeto.AddComponent<TrailRenderer>();
        TrailRenderer linea = efecto.estela;
        linea.sharedMaterial = efecto.material;
        linea.time = Mathf.Max(0.01f, duracion);
        linea.minVertexDistance = 0.03f;
        linea.widthMultiplier = Mathf.Max(0.01f, ancho);
        linea.widthCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        linea.startColor = color;
        linea.endColor = new Color(color.r, color.g, color.b, 0f);
        linea.alignment = LineAlignment.View;
        linea.shadowCastingMode = ShadowCastingMode.Off;
        linea.receiveShadows = false;
        linea.numCapVertices = 3;
        linea.emitting = true;
        linea.AddPosition(origen.position);
    }

    void LateUpdate()
    {
        if (finalizada || estela == null) return;
        if (proyectil != null)
        {
            transform.position = proyectil.position;
            return;
        }
        finalizada = true;
        estela.emitting = false;
        Destroy(gameObject, estela.time);
    }

    void OnDestroy()
    {
        if (material != null) Destroy(material);
    }
}
