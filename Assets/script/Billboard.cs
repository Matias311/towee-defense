using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Header("Camara")]
    [Tooltip("Si lo dejas vacio, usa automaticamente la Main Camera")]
    public Camera camaraObjetivo;

    [Header("Opciones")]
    [Tooltip("Si esta tildado, solo rota en el eje Y (el objeto se mantiene 'parado', como los enemigos de Doom). Si esta destildado, mira a la camara en todos los ejes.")]
    public bool soloEjeY = true;

    void Start()
    {
        if (camaraObjetivo == null)
        {
            camaraObjetivo = Camera.main;
        }
    }

    void LateUpdate()
    {
        if (camaraObjetivo == null) return;

        if (soloEjeY)
        {
            // Miramos hacia la camara pero ignorando la altura (Y),
            // asi el objeto queda "parado" siempre en vez de inclinarse
            Vector3 direccion = camaraObjetivo.transform.position - transform.position;
            direccion.y = 0f;

            if (direccion != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direccion) * Quaternion.Euler(0f, 180f, 0f);
            }
        }
        else
        {
            // Mira directamente a la camara en todos los ejes
            transform.LookAt(camaraObjetivo.transform);
            transform.Rotate(0f, 180f, 0f);
        }
    }
}