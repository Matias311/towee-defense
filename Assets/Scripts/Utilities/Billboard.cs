using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Header("Camara")]
    [Tooltip("Si lo dejas vacio, usa automaticamente la Main Camera")]
    public Camera camaraObjetivo;

    [Header("Opciones")]
    [Tooltip("Si esta tildado, solo rota en el eje Y (el objeto se mantiene 'parado', como los enemigos de Doom). Si esta destildado, mira a la camara en todos los ejes.")]
    public bool soloEjeY = true;

    void LateUpdate()
    {
        // En vez de buscar la camara solo una vez en Start,
        // la revalidamos cada frame si se perdió la referencia.
        if (camaraObjetivo == null)
        {
            camaraObjetivo = Camera.main;
            if (camaraObjetivo == null)
            {
                Debug.LogWarning("Billboard: no se encontró ninguna cámara con tag 'MainCamera'. " + gameObject.name + " no puede rotar.");
                return;
            }
        }

        if (soloEjeY)
        {
            Vector3 direccion = camaraObjetivo.transform.position - transform.position;
            direccion.y = 0f;

            if (direccion != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direccion) * Quaternion.Euler(0f, 180f, 0f);
            }
        }
        else
        {
            transform.LookAt(camaraObjetivo.transform);
            transform.Rotate(0f, 180f, 0f);
        }
    }
}