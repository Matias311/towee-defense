using UnityEngine;

/// <summary>Modelos con huesos por rareza; independientes del perfil de combate de cada oferta.</summary>
[CreateAssetMenu(fileName = "AparienciasTorretas", menuName = "Tower Defense/Apariencias de torretas")]
public class AparienciasTorretas : ScriptableObject
{
    public GameObject normal;
    public GameObject rara;
    public GameObject epica;
    public GameObject legendaria;
    [Min(.1f)] public float altura = 2.2f;

    public GameObject Obtener(RarezaCarta rareza)
    {
        switch (rareza)
        {
            case RarezaCarta.Rara: return rara;
            case RarezaCarta.Epica: return epica;
            case RarezaCarta.Legendaria: return legendaria;
            default: return normal;
        }
    }

    public GameObject Crear(RarezaCarta rareza, string nombre)
    {
        GameObject prefab = Obtener(rareza);
        if (prefab == null) return null;
        var raiz = new GameObject(nombre);
        GameObject modelo = Instantiate(prefab, raiz.transform, false);
        modelo.name = prefab.name;
        // Normalizar el tamaño conservando el rig completo del PSB y apoyando los pies en el suelo.
        SpriteRenderer[] partes = modelo.GetComponentsInChildren<SpriteRenderer>();
        bool hayLimites = false;
        Bounds limites = default;
        foreach (SpriteRenderer parte in partes)
        {
            if (!parte.enabled || parte.sprite == null) continue;
            if (!hayLimites) { limites = parte.bounds; hayLimites = true; }
            else limites.Encapsulate(parte.bounds);
        }
        if (hayLimites && limites.size.y > .001f)
        {
            float escala = altura / limites.size.y;
            modelo.transform.localScale *= escala;
            Vector3 apoyo = new Vector3(limites.center.x, limites.min.y, limites.center.z);
            modelo.transform.localPosition -= apoyo * escala;
        }
        if (modelo.GetComponent<Billboard>() == null) modelo.AddComponent<Billboard>();
        return raiz;
    }
}
