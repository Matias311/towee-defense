using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab del enemigo")]
    [Tooltip("Arrastra aqui el Prefab de enemy-cube")]
    public GameObject enemyPrefab;

    [Header("Waypoints")]
    [Tooltip("Arrastra aqui los mismos waypoints en el mismo orden que usaste en EnemyMovement")]
    public List<Transform> waypoints = new List<Transform>();

    [Header("Configuracion de oleada")]
    [Tooltip("Si esta tildado, los enemigos no dejan de salir nunca (ignora Cantidad Enemigos)")]
    public bool spawnInfinito = true;

    [Tooltip("Cuantos enemigos van a salir en total (solo se usa si Spawn Infinito esta destildado)")]
    public int cantidadEnemigos = 10;

    [Tooltip("Tiempo en segundos entre cada enemigo que sale")]
    public float tiempoEntreEnemigos = 1.5f;

    [Tooltip("Tiempo de espera antes de que salga el primer enemigo")]
    public float tiempoInicial = 2f;

    [Header("Dificultad progresiva")]
    [Tooltip("Velocidad del primer enemigo que sale")]
    public float velocidadInicial = 3f;

    [Tooltip("Cuanto aumenta la velocidad con cada enemigo nuevo que sale")]
    public float incrementoVelocidad = 0.05f;

    [Tooltip("Velocidad maxima que puede alcanzar (para que no se vuelva imposible)")]
    public float velocidadMaxima = 10f;

    private float temporizador;
    private int enemigosCreados = 0;
    private bool spawneando = false;

    void Start()
    {
        temporizador = tiempoInicial;
        spawneando = true;
    }

    void Update()
    {
        if (!spawneando) return;

        temporizador -= Time.deltaTime;

        if (temporizador <= 0f)
        {
            SpawnEnemigo();
            temporizador = tiempoEntreEnemigos;
            enemigosCreados++;

            if (!spawnInfinito && enemigosCreados >= cantidadEnemigos)
            {
                spawneando = false;
            }
        }
    }

    void SpawnEnemigo()
    {
        if (enemyPrefab == null || waypoints.Count == 0) return;

        // Crear una copia del prefab en la posicion del primer waypoint
        GameObject nuevoEnemigo = Instantiate(enemyPrefab, waypoints[0].position, Quaternion.identity);

        // Pasarle la lista de waypoints al script de movimiento del nuevo enemigo
        EnemyMovement movimiento = nuevoEnemigo.GetComponent<EnemyMovement>();
        if (movimiento != null)
        {
            movimiento.waypoints = waypoints;

            // Calcular la velocidad progresiva: aumenta un poco con cada enemigo, sin pasar el maximo
            float velocidadCalculada = velocidadInicial + (incrementoVelocidad * enemigosCreados);
            movimiento.speed = Mathf.Min(velocidadCalculada, velocidadMaxima);
        }
    }
}