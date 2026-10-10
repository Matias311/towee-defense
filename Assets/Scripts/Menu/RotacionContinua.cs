using UnityEngine;
 
public class RotacionContinua : MonoBehaviour
{
    [Tooltip("Velocidad de giro en grados por segundo. Un numero negativo gira en sentido contrario.")]
    public float velocidadGiro = 20f;
 
    void Update()
    {
        // Usamos unscaledDeltaTime en vez de deltaTime para que el giro
        // siga funcionando aunque el juego este en pausa (Time.timeScale = 0)
        transform.Rotate(0f, 0f, velocidadGiro * Time.unscaledDeltaTime);
    }
}
 