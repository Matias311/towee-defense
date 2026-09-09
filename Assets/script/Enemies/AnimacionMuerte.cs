using UnityEngine;

public class AutoDestruirExplosion : MonoBehaviour
{
    [Tooltip("Tiempo en segundos antes de destruir este objeto (duracion aprox de la animacion)")]
    public float duracion = 1f;

    void Start()
    {
        Destroy(gameObject, duracion);
    }
}