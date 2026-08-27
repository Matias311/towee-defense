using System.Collections.Generic;
using UnityEngine;

public enum EnemyType {
    Base,
    Special,
    UltraPro,
    BossL1,
    BossL2,
    BossL3,
    BossL4
}

[System.Serializable]
public class EnemyProbability {
    public EnemyType type;
    public GameObject prefab;
    public float baseProbability;
}

public class EnemySpawner : MonoBehaviour {
    [Header("Prefabs de enemigos")]
    [Tooltip("Arrastra los 6 prefabs aquí: enemy-cube, enemy-cube-special, enemy-cube-special 4, enemy-cube-boss (y variantes)")]
    public List<EnemyProbability> enemyProbabilities;

    [Header("Waypoints")]
    [Tooltip("Arrastra aqui los mismos waypoints en el mismo orden que usaste en EnemyMovement")]
    public List<Transform> waypoints = new List<Transform>();

    [Header("Configuracion de oleada")]
    [Tooltip("Si esta tildado, los enemigos no dejan de salir nunca (ignora Cantidad Enemigos)")]
    public bool spawnInfinito = true;

    [Tooltip("Cuantos enemigos van a salir en total (solo se usa si Spawn Infinito esta destildado)")]
    public int cantidadEnemigos = 15;

    [Tooltip("Tiempo en segundos entre cada enemigo que sale")]
    public float tiempoEntreEnemigos = 1.5f;

    [Tooltip("Cantidad de enemigos por oleada (para modo infinito)")]
    public int enemigosPorOleada = 15;

    [Tooltip("Tiempo de espera antes de que salga el primer enemigo")]
    public float tiempoInicial = 2f;

    [Header("Dificultad progresiva")]
    [Tooltip("Velocidad del primer enemigo que sale")]
    public float velocidadInicial = 3f;

    [Tooltip("Cuanto aumenta la velocidad con cada enemigo nuevo que sale")]
    public float incrementoVelocidad = 0.05f;

    [Tooltip("Velocidad maxima que puede alcanzar (para que no se vuelva imposible)")]
    public float velocidadMaxima = 10f;

    [Header("Configuracion de oleadas")]
    public int oleadaActual = 1;

    private float temporizador;
    private int enemigosCreados = 0;
    private bool spawneando = false;

    void Start() {
        temporizador = tiempoInicial;
        spawneando = true;
        // Asignar prefabs por defecto si no estan asignados
        AssignDefaultPrefabs();
    }

    void AssignDefaultPrefabs() {
        if (enemyProbabilities == null || enemyProbabilities.Count == 0) {
            enemyProbabilities = new List<EnemyProbability> {
                new EnemyProbability { type = EnemyType.Base, prefab = null, baseProbability = 90f },
                new EnemyProbability { type = EnemyType.Special, prefab = null, baseProbability = 5f },
                new EnemyProbability { type = EnemyType.UltraPro, prefab = null, baseProbability = 4f },
                new EnemyProbability { type = EnemyType.BossL1, prefab = null, baseProbability = 1f }
            };
            Debug.Log("[SPAWNER] Default prefabs asignados a null - ahora asigna tus prefabs en el Inspector");
        }
    }

    EnemyType SeleccionarTipoEnemigo() {
        float randomValue = Random.value * 100f;
        float acumulador = 0f;

        foreach (var prob in enemyProbabilities) {
            acumulador += prob.baseProbability;
            if (randomValue <= acumulador) {
                return prob.type;
            }
        }
        return EnemyType.Base;
    }

    float ObtenerProbabilidadJefeActual() {
        // En la 4ta oleada, boss obligatorio
        if (oleadaActual >= 4) {
            return 100f;
        }
        // Probabilidad base que incrementa con el nivel
        float progreso = oleadaActual / 4f;
        return Mathf.Clamp(progreso, 0f, 1f);
    }

    void Update() {
        if (!spawneando) return;

        temporizador -= Time.deltaTime;

        if (temporizador <= 0f) {
            Debug.Log($"[SPAWNER] Timer done! Oleada: {oleadaActual}, Enemies: {enemigosCreados}, spawneando: {spawneando}");
            SpawnEnemigo();
            temporizador = tiempoEntreEnemigos;
            enemigosCreados++;

            if (!spawnInfinito && enemigosCreados >= cantidadEnemigos) {
                spawneando = false;
                Debug.Log("[SPAWNER] Detenido: reached cantidadEnemigos");
            } else if (spawnInfinito && enemigosCreados % enemigosPorOleada == 0 && enemigosCreados > 0) {
                oleadaActual++;
                Debug.Log("[SPAWNER] Oleada avanzada a: " + oleadaActual);
            } else if (spawnInfinito) {
                Debug.Log($"[SPAWNER] Continuando... timer={temporizador:F1}/{tiempoEntreEnemigos:F1}");
            }
        }
    }

    void SpawnEnemigo() {
            Debug.Log($"[SPAWN] Iniciando spawn. Oleada: {oleadaActual}, EnemiesCreated: {enemigosCreados}, ProbCount: {enemyProbabilities.Count}, Waypoints: {waypoints.Count}");

            if (enemyProbabilities == null) {
                Debug.LogError("[SPAWNER] enemyProbabilities es null!");
                return;
            }
            if (enemyProbabilities.Count == 0) {
                Debug.LogError("[SPAWNER] enemyProbabilities lista vacía! Asignando defaults...");
                AssignDefaultPrefabs();
            }
            if (waypoints.Count == 0) {
                Debug.LogError("[SPAWNER] waypoints lista vacía! No se pueden spawnear.");
                return;
            }

            // Verificar si hay suficiente información para spawnear
            bool hasValidPrefabs = false;
            foreach (var probability in enemyProbabilities) {
                if (probability != null && probability.prefab != null) {
                    hasValidPrefabs = true;
                    break;
                }
            }
            if (!hasValidPrefabs) {
                Debug.LogError("[SPAWNER] Ningún prefab asignado en enemyProbabilities! Arrastra los prefabs en el Inspector.");
                return;
            }

            // Verificar si es oleada especial (4ta oleada = boss obligatorio)
            bool bossOleada = (oleadaActual >= 4);

            EnemyType tipoSeleccionado = SeleccionarTipoEnemigo();

            // Si es la 4ta oleada o boss obligatorio, asegurar boss
            if (bossOleada) {
                // Lógica para jefe con sub-niveles según oleada
                float totalBossProb = 1f; // Siempre sale boss
                float subProb = 0.40f; // Default boss nivel 1

                if (oleadaActual >= 4) {
                    // Distribución según oleada:
                    // Oleada 4: 40% nivel 1, 20% nivel 2, 10% nivel 3, 30% nivel 4... ajustado
                    int oleadaBoss = oleadaActual - 3; // 4ta -> 1er nivel de boss, etc.
                    // 4ta -> 100% de probabilidad de boss, pero si es infinito, que sea (4 * 2) % 2  (para que no se pase de 4)
                    if (oleadaBoss >= 4) oleadaBoss = 4;
                    switch (oleadaBoss) {
                        case 1: subProb = 0.40f; break;
                        case 2: subProb = 0.20f; break;
                        case 3: subProb = 0.10f; break;
                        case 4: subProb = 0.30f; break;
                    }
                }

                // Seleccionar sub-tipo de boss según probabilidad
                float rand = Random.value * 100f;
                if (rand <= 0.40f) tipoSeleccionado = EnemyType.BossL1;
                else if (rand <= 0.40f + 0.20f) tipoSeleccionado = EnemyType.BossL2;
                else if (rand <= 0.40f + 0.20f + 0.10f) tipoSeleccionado = EnemyType.BossL3;
                else tipoSeleccionado = EnemyType.BossL4;
            }

            // Encontrar el prefab correspondiente al tipo seleccionado
            GameObject prefabSeleccionado = null;
            foreach (var prob in enemyProbabilities) {
                if (prob.type == tipoSeleccionado) {
                    prefabSeleccionado = prob.prefab;
                    break;
                }
            }

            if (prefabSeleccionado == null) {
                Debug.LogError("[SPAWNER] prefabSeleccionado es null! Revisa que los prefabs estén asignados en enemyProbabilities.");
                return;
            }

            // Crear una copia del prefab en la posicion del primer waypoint
            GameObject nuevoEnemigo = Instantiate(prefabSeleccionado, waypoints[0].position, Quaternion.identity);
            Debug.Log($"[SPAWNER] Enemy instantiated: {nuevoEnemigo.name}");

            // Pasarle la lista de waypoints al script de movimiento del nuevo enemigo
            EnemyMovement movimiento = nuevoEnemigo.GetComponent<EnemyMovement>();
            if (movimiento != null) {
                movimiento.waypoints = waypoints;

                // Calcular la velocidad progresiva: aumenta un poco con cada enemigo, sin pasar el maximo
                float velocidadCalculada = velocidadInicial + (incrementoVelocidad * enemigosCreados);
                movimiento.speed = Mathf.Min(velocidadCalculada, velocidadMaxima);
            }
    }}