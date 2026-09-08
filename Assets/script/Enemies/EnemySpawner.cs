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
    [Tooltip("Probabilidad calculada para la wave actual")]
    public float probabilidadActual;
    [Tooltip("Estadisticas que usaran los enemigos de este tipo")]
    public EnemyStatsData estadisticas;
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

    [Tooltip("Activa la progresion de 3 niveles con 4 waves y un boss por nivel")]
    public bool modoNiveles = true;

    [Tooltip("Cuantos enemigos van a salir en total (solo se usa si Spawn Infinito esta destildado)")]
    public int cantidadEnemigos = 15;

    [Tooltip("Tiempo en segundos entre cada enemigo que sale")]
    public float tiempoEntreEnemigos = 1.5f;

    [Tooltip("Cantidad de enemigos por oleada (para modo infinito)")]
    public int enemigosPorOleada = 15;

    [Tooltip("Tiempo de espera antes de que salga el primer enemigo")]
    public float tiempoInicial = 2f;

    [Tooltip("Tiempo de espera entre waves")]
    public float tiempoEntreOleadas = 10f;

    [Header("Dificultad progresiva")]
    [Tooltip("Velocidad del primer enemigo que sale")]
    public float velocidadInicial = 3f;

    [Tooltip("Cuanto aumenta la velocidad con cada enemigo nuevo que sale")]
    public float incrementoVelocidad = 0.05f;

    [Tooltip("Velocidad maxima que puede alcanzar (para que no se vuelva imposible)")]
    public float velocidadMaxima = 10f;

    [Header("Escalado de velocidad infinita")]
    [Tooltip("Aumento de la velocidad inicial por cada wave infinita")]
    public float incrementoVelocidadInicialPorOleada = 0.25f;

    [Tooltip("Aumento de la velocidad maxima por cada wave infinita")]
    public float incrementoVelocidadMaximaPorOleada = 0.5f;

    [Tooltip("Aumento del incremento de velocidad por cada wave infinita")]
    public float incrementoIncrementoVelocidadPorOleada = 0.01f;

    [Header("Dificultad de oleadas infinitas")]
    [Tooltip("Porcentaje de la probabilidad base que se redistribuye por cada wave")]
    [Range(0f, 1f)]
    public float reduccionProbabilidadBasePorOleada = 0.1f;

    [Tooltip("Parte de la probabilidad redistribuida que reciben los bosses por cada wave infinita")]
    [Range(0f, 1f)]
    public float incrementoProbabilidadBossPorOleada = 0.1f;

    [Tooltip("Porcentaje adicional de dificultad por cada nivel")]
    [Range(0f, 1f)]
    public float incrementoDificultadPorNivel = 0.15f;

    [Header("Configuracion de oleadas")]
    [Range(1, 3)]
    public int nivelActual = 1;
    public int oleadaActual = 1;

    private float temporizador;
    private int enemigosCreados = 0;
    private bool spawneando = false;
    private bool bossDeOleadaCuatroCreado = false;
    private bool nivelCompletado = false;

    void Start() {
        // Asignar prefabs por defecto si no estan asignados
        AssignDefaultPrefabs();
        AplicarProbabilidadesPorDefecto();
        IniciarNivel(nivelActual);
    }

    public void IniciarNivel(int nivel) {
        nivelActual = Mathf.Clamp(nivel, 1, 3);
        oleadaActual = 1;
        enemigosCreados = 0;
        bossDeOleadaCuatroCreado = false;
        nivelCompletado = false;
        temporizador = tiempoInicial;
        spawneando = true;
        Debug.Log($"[SPAWNER] Nivel {nivelActual} iniciado. Enemigos por wave: {ObtenerEnemigosPorOleada()}, velocidad inicial: {ObtenerVelocidadInicial():F2}");
    }

    public void SeleccionarNivel(int nivel) {
        IniciarNivel(nivel);
    }

    public void AvanzarAlSiguienteNivel() {
        if (nivelActual >= 3) {
            Debug.Log("[SPAWNER] Se completo el nivel maximo.");
            nivelCompletado = false;
            return;
        }

        IniciarNivel(nivelActual + 1);
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

    void AplicarProbabilidadesPorDefecto() {
        foreach (var prob in enemyProbabilities) {
            if (prob == null || prob.prefab == null || prob.baseProbability > 0f) {
                continue;
            }

            prob.baseProbability = ObtenerProbabilidadPorDefecto(prob.type);
        }
    }

    float ObtenerProbabilidadPorDefecto(EnemyType type) {
        switch (type) {
            case EnemyType.Base:
                return 85f;
            case EnemyType.Special:
                return 10f;
            case EnemyType.UltraPro:
                return 5f;
            case EnemyType.BossL1:
                return 0.5f;
            case EnemyType.BossL2:
                return 0.35f;
            case EnemyType.BossL3:
                return 0.2f;
            case EnemyType.BossL4:
                return 0.1f;
            default:
                return 0f;
        }
    }

    int ObtenerEnemigosPorOleada() {
        if (!modoNiveles) {
            return Mathf.Max(1, enemigosPorOleada);
        }

        return Mathf.Max(1, enemigosPorOleada + (nivelActual - 1) * 2);
    }

    float ObtenerVelocidadInicial() {
        float velocidadPorNivel = velocidadInicial
            * (1f + incrementoDificultadPorNivel * (nivelActual - 1));

        if (!modoNiveles) {
            velocidadPorNivel += incrementoVelocidadInicialPorOleada
                * Mathf.Max(0, oleadaActual - 1);
        }

        return velocidadPorNivel;
    }

    float ObtenerVelocidadMaxima() {
        if (modoNiveles) {
            return velocidadMaxima;
        }

        return velocidadMaxima
            + incrementoVelocidadMaximaPorOleada * Mathf.Max(0, oleadaActual - 1);
    }

    float ObtenerIncrementoVelocidad() {
        if (modoNiveles) {
            return incrementoVelocidad;
        }

        return incrementoVelocidad
            + incrementoIncrementoVelocidadPorOleada * Mathf.Max(0, oleadaActual - 1);
    }

    EnemyType SeleccionarTipoEnemigo() {
        float probabilidadBaseTotal = 0f;
        float probabilidadNoBaseTotal = 0f;
        float probabilidadBossTotal = 0f;

        foreach (var prob in enemyProbabilities) {
            if (prob == null) continue;

            if (prob.type == EnemyType.Base) {
                probabilidadBaseTotal += Mathf.Max(0f, prob.baseProbability);
            } else {
                probabilidadNoBaseTotal += Mathf.Max(0f, prob.baseProbability);
                if (EsBoss(prob.type)) {
                    probabilidadBossTotal += Mathf.Max(0f, prob.baseProbability);
                }
            }
        }

        float randomValue = Random.value * (probabilidadBaseTotal + probabilidadNoBaseTotal);
        float acumulador = 0f;

        foreach (var prob in enemyProbabilities) {
            if (prob == null) continue;

            prob.probabilidadActual = ObtenerProbabilidadAjustada(
                prob,
                probabilidadBaseTotal,
                probabilidadNoBaseTotal,
                probabilidadBossTotal
            );
            acumulador += prob.probabilidadActual;
            if (randomValue <= acumulador) {
                return prob.type;
            }
        }
        return EnemyType.Base;
    }

    float ObtenerProbabilidadAjustada(
        EnemyProbability prob,
        float probabilidadBaseTotal,
        float probabilidadNoBaseTotal,
        float probabilidadBossTotal
    ) {
        float probabilidadOriginal = Mathf.Max(0f, prob.baseProbability);

        if ((!modoNiveles && !spawnInfinito) || probabilidadBaseTotal <= 0f) {
            return probabilidadOriginal;
        }

        int wavesDeProgreso = modoNiveles
            ? oleadaActual - 1
            : Mathf.Max(0, oleadaActual - 4);
        float progreso = Mathf.Clamp01(
            (modoNiveles ? incrementoDificultadPorNivel * (nivelActual - 1) : 0f)
            + reduccionProbabilidadBasePorOleada * wavesDeProgreso
        );

        if (prob.type == EnemyType.Base) {
            return probabilidadOriginal * (1f - progreso);
        }

        if (probabilidadNoBaseTotal <= 0f) {
            return probabilidadOriginal;
        }

        float probabilidadRedistribuida = probabilidadBaseTotal * progreso;
        float probabilidadNoBossTotal = probabilidadNoBaseTotal - probabilidadBossTotal;

        if (!modoNiveles && spawnInfinito && probabilidadBossTotal > 0f) {
            float progresoBoss = Mathf.Clamp01(
                incrementoProbabilidadBossPorOleada * wavesDeProgreso
            );

            if (EsBoss(prob.type)) {
                return probabilidadOriginal
                    + probabilidadRedistribuida
                    * progresoBoss
                    * (probabilidadOriginal / probabilidadBossTotal);
            }

            if (probabilidadNoBossTotal > 0f) {
                return probabilidadOriginal
                    + probabilidadRedistribuida
                    * (1f - progresoBoss)
                    * (probabilidadOriginal / probabilidadNoBossTotal);
            }
        }

        return probabilidadOriginal
            + probabilidadRedistribuida * (probabilidadOriginal / probabilidadNoBaseTotal);
    }

    bool EsBoss(EnemyType type) {
        return type == EnemyType.BossL1
            || type == EnemyType.BossL2
            || type == EnemyType.BossL3
            || type == EnemyType.BossL4;
    }

    EnemyType SeleccionarBoss() {
        EnemyType[] bosses = {
            EnemyType.BossL1,
            EnemyType.BossL2,
            EnemyType.BossL3,
            EnemyType.BossL4
        };

        int bossIndex = Mathf.Clamp(nivelActual - 1, 0, bosses.Length - 1);
        EnemyType bossDelNivel = bosses[bossIndex];

        foreach (var prob in enemyProbabilities) {
            if (prob != null && prob.type == bossDelNivel && prob.prefab != null) {
                return bossDelNivel;
            }
        }

        foreach (EnemyType boss in bosses) {
            foreach (var prob in enemyProbabilities) {
                if (prob != null && prob.type == boss && prob.prefab != null) {
                    return boss;
                }
            }
        }

        return EnemyType.BossL1;
    }

    void EliminarEnemigosActivos() {
        EnemyMovement[] enemigos = FindObjectsByType<EnemyMovement>();

        foreach (EnemyMovement enemigo in enemigos) {
            Destroy(enemigo.gameObject);
        }

        Debug.Log($"[SPAWNER] Enemigos eliminados al comenzar la wave {oleadaActual}: {enemigos.Length}");
    }

    void Update() {
        if (nivelCompletado) return;

        if (!spawneando) return;

        temporizador -= Time.deltaTime;

        if (temporizador <= 0f) {
            Debug.Log($"[SPAWNER] Timer done! Oleada: {oleadaActual}, Enemies: {enemigosCreados}, spawneando: {spawneando}");
            SpawnEnemigo();

            if (nivelCompletado) {
                return;
            }

            enemigosCreados++;
            temporizador = tiempoEntreEnemigos;
            TowerPlacer towerPlacer = FindAnyObjectByType<TowerPlacer>();
            if (towerPlacer != null) {
                towerPlacer.ActualizarWave(oleadaActual);
            }

            if (!modoNiveles && !spawnInfinito && enemigosCreados >= cantidadEnemigos) {
                spawneando = false;
                Debug.Log("[SPAWNER] Detenido: reached cantidadEnemigos");
            } else if (modoNiveles && enemigosCreados % ObtenerEnemigosPorOleada() == 0 && enemigosCreados > 0) {
                oleadaActual++;
                temporizador = tiempoEntreOleadas;
                Debug.Log("[SPAWNER] Oleada avanzada a: " + oleadaActual);
            } else if (!modoNiveles && spawnInfinito && enemigosCreados % ObtenerEnemigosPorOleada() == 0 && enemigosCreados > 0) {
                oleadaActual++;
                temporizador = tiempoEntreOleadas;
                Debug.Log("[SPAWNER] Oleada avanzada a: " + oleadaActual);
            } else if (modoNiveles || spawnInfinito) {
                Debug.Log($"[SPAWNER] Continuando... timer={temporizador:F1}/{tiempoEntreEnemigos:F1}");
            }
        }
    }

    void SpawnEnemigo() {
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

            Debug.Log($"[SPAWN] Iniciando spawn. Oleada: {oleadaActual}, EnemiesCreated: {enemigosCreados}, ProbCount: {enemyProbabilities.Count}, Waypoints: {waypoints.Count}");

            EnemyType tipoSeleccionado = SeleccionarTipoEnemigo();

            // La wave 4 tiene un unico evento de boss y limpia los enemigos anteriores.
            bool eventoBoss = (modoNiveles || spawnInfinito)
                && oleadaActual == 4
                && !bossDeOleadaCuatroCreado;

            if (eventoBoss) {
                EliminarEnemigosActivos();
                tipoSeleccionado = SeleccionarBoss();
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

            if (eventoBoss) {
                bossDeOleadaCuatroCreado = true;
                if (modoNiveles) {
                    nivelCompletado = true;
                    spawneando = false;
                    Debug.Log($"[SPAWNER] Boss del nivel {nivelActual} creado. Esperando confirmacion de muerte para avanzar.");
                } else {
                    Debug.Log("[SPAWNER] Boss de la wave 4 creado. El modo infinito continuara.");
                }
            }

            // Pasarle la lista de waypoints al script de movimiento del nuevo enemigo
            EnemyMovement movimiento = nuevoEnemigo.GetComponent<EnemyMovement>();
            if (movimiento != null) {
                movimiento.waypoints = waypoints;

                // Calcular la velocidad progresiva: aumenta un poco con cada enemigo, sin pasar el maximo
                float velocidadCalculada = ObtenerVelocidadInicial()
                    + (ObtenerIncrementoVelocidad() * enemigosCreados);
                movimiento.speed = Mathf.Min(velocidadCalculada, ObtenerVelocidadMaxima());
            }

            EnemyStats estadisticas = nuevoEnemigo.GetComponent<EnemyStats>();
            if (estadisticas == null) {
                estadisticas = nuevoEnemigo.AddComponent<EnemyStats>();
            }

            EnemyStatsData datosSeleccionados = null;
            foreach (var prob in enemyProbabilities) {
                if (prob != null && prob.type == tipoSeleccionado) {
                    datosSeleccionados = prob.estadisticas;
                    break;
                }
            }

            estadisticas.Configurar(
                tipoSeleccionado,
                nivelActual,
                oleadaActual,
                datosSeleccionados
            );
            Debug.Log(
                $"[SPAWNER] Stats aplicadas: tipo={tipoSeleccionado}, " +
                $"vida={estadisticas.vidaMaxima:F1}, daño={estadisticas.daño:F1}, " +
                $"defensa={estadisticas.defensa:F1}, pasiva={estadisticas.pasiva}"
            );
    }}