using UnityEngine;

public class MaterialRecolectable : MonoBehaviour
{
    public int valor = 1;
    public float tiempoDeVida = 30f; // 0 = no desaparece

    void Start()
    {
        if (tiempoDeVida > 0f) Destroy(gameObject, tiempoDeVida);
    }

    public void Recolectar()
    {
        if (InventarioMateriales.Instancia != null)
            InventarioMateriales.Instancia.Sumar(valor);
        Destroy(gameObject);
    }
}