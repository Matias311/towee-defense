//librerias necesarias
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Waypoints")]
    [Tooltip("Arrastra aqui los objetos waiponts en orden: waiponts, waiponts (1), waiponts (2), etc.")]
    // Lista de waypoints por los que el enemigo se moverá
    public List<Transform> waypoints = new List<Transform>();

    [Header("Movimiento")]
    // Velocidad de movimiento del enemigo
    public float speed = 3f;
    // Distancia mínima para considerar que ha llegado al waypoint
    public float distanciaLlegada = 0.1f;

    [Header("Rotación")]
    [Tooltip("Activar para enemigos 3D tipo cubo (rotan hacia donde caminan). Desactivar para enemigos con sprite 2D, ya que el Billboard.cs se encarga de la rotación hacia la cámara.")]
    public bool rotarHaciaMovimiento = true;

    [Header("Al llegar al final")]
    // Indica si el enemigo debe destruirse al llegar al último waypoint
    public bool destruirAlLlegar = true;

    // Indice del waypoint actual al que el enemigo se dirige
    private int indiceActual = 0;

    // Método Start se llama al inicio del juego
    void Start()
    {
        if (waypoints.Count > 0)
        {
            transform.position = waypoints[0].position;
        }
    }

    // Método Update se llama una vez por frame
    void Update()
    {
        if (waypoints.Count == 0) return;

        // Si ya llego al ultimo waypoint
        if (indiceActual >= waypoints.Count)
        {
            if (destruirAlLlegar)
            {
                Destroy(gameObject);
            }
            return;
        }

        // Obtener el waypoint actual al que se dirige el enemigo
        Transform objetivo = waypoints[indiceActual];

        // Mover hacia el waypoint actual
        transform.position = Vector3.MoveTowards(
            transform.position,
            objetivo.position,
            speed * Time.deltaTime
        );

        // Rotar el objeto para que "mire" hacia donde va
        // Solo se aplica si rotarHaciaMovimiento está activado (enemigos 3D tipo cubo).
        // En enemigos con sprite 2D esto debe estar desactivado en el Inspector,
        // ya que Billboard.cs es quien controla la rotación hacia la cámara.
        if (rotarHaciaMovimiento)
        {
            Vector3 direccion = objetivo.position - transform.position;
            if (direccion != Vector3.zero)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, 10f * Time.deltaTime);
            }
        }

        // Si llego al waypoint, avanzar al siguiente
        if (Vector3.Distance(transform.position, objetivo.position) < distanciaLlegada)
        {
            indiceActual++;
        }
    }
}