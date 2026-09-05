# Fórmulas y porcentajes del `EnemySpawner`

Este documento describe el cálculo que usa `Assets/script/EnemySpawner.cs` para
seleccionar enemigos, avanzar oleadas y aumentar la dificultad. Los valores de
la configuración activa corresponden al objeto `EnemySpawner` de
`Assets/Scenes/etapa1.unity`.

## Configuración activa

| Campo | Valor | Efecto |
| --- | ---: | --- |
| `spawnInfinito` | `true` | El spawner no se detiene por `cantidadEnemigos`. |
| `modoNiveles` | `false` | Se usa el modo infinito; no se aplica el incremento por nivel. |
| `enemigosPorOleada` | `5` | Cada oleada normal contiene 5 enemigos. |
| `tiempoInicial` | `2 s` | Espera antes del primer spawn. |
| `tiempoEntreEnemigos` | `1.5 s` | Espera entre spawns normales. |
| `tiempoEntreOleadas` | `10 s` | Espera al terminar una oleada. |
| `velocidadInicial` | `5` | Velocidad del primer enemigo de la oleada 1. |
| `incrementoVelocidad` | `0.05` | Incremento por cada enemigo creado. |
| `velocidadMaxima` | `10` | Límite de velocidad en el modo infinito antes del escalado de oleada. |
| `reduccionProbabilidadBasePorOleada` | `32.5%` | Parte de la probabilidad de `Base` que se redistribuye desde la oleada 5. |
| `incrementoProbabilidadBossPorOleada` | `10%` | Parte de la redistribución que pasa a bosses por oleada infinita. |
| `incrementoVelocidadInicialPorOleada` | `0.25` | Aumento de velocidad inicial por oleada infinita. |
| `incrementoVelocidadMaximaPorOleada` | `0.5` | Aumento del límite de velocidad por oleada infinita. |
| `incrementoIncrementoVelocidadPorOleada` | `0.01` | Aumento del incremento por enemigo en cada oleada infinita. |

## Pesos base de spawn

`baseProbability` se trata como un **peso**, no como un porcentaje que deba
sumar exactamente 100. La escena tiene estos pesos efectivos:

| Tipo | Peso base | Porcentaje aproximado en la oleada 1 |
| --- | ---: | ---: |
| `Base` | 85 | 84.03% |
| `Special` | 10 | 9.89% |
| `UltraPro` | 5 | 4.94% |
| `BossL1` | 0.50 | 0.49% |
| `BossL2` | 0.35 | 0.35% |
| `BossL3` | 0.20 | 0.20% |
| `BossL4` | 0.10 | 0.10% |
| **Total** | **101.15** | **100%** |

La suma es `101.15` porque los valores serializados en cero se completan con
los valores predeterminados cuando el prefab existe. La escena contiene dos
entradas `UltraPro`, pero solo una tiene prefab asignado; la entrada sin prefab
mantiene peso cero y no cambia el resultado.

El porcentaje real de un tipo se calcula así:

```text
porcentaje = pesoAjustadoDelTipo / sumaDePesosAjustados * 100
```

La selección genera un número aleatorio dentro de la suma de pesos y recorre
la tabla acumulando cada peso. Por eso el orden de las entradas también forma
parte del comportamiento cuando los pesos tienen valores límite.

## Redistribución de probabilidades en modo infinito

En modo infinito, la dificultad de probabilidad comienza en la oleada 5:

```text
wavesDeProgreso = max(0, oleadaActual - 4)
progresoBase = clamp(0.325 * wavesDeProgreso, 0, 1)
probabilidadRedistribuida = pesoBaseTotal * progresoBase
pesoAjustadoBase = pesoBase * (1 - progresoBase)
```

Con la configuración actual, `pesoBaseTotal = 85`. La redistribución máxima
es, por tanto, 85 puntos de peso. La parte redistribuida se reparte entre
enemigos normales y bosses:

```text
progresoBoss = clamp(0.10 * wavesDeProgreso, 0, 1)

redistribucionNoBoss =
    probabilidadRedistribuida * (1 - progresoBoss)

redistribucionBoss =
    probabilidadRedistribuida * progresoBoss
```

Cada grupo conserva la proporción de sus pesos originales:

```text
extraTipoNoBoss = redistribucionNoBoss
                 * (pesoTipo / pesoTotalNoBoss)

extraTipoBoss = redistribucionBoss
                * (pesoTipo / pesoTotalBoss)

pesoAjustadoTipo = pesoOriginalTipo + extraTipo
```

Para esta escena:

```text
pesoTotalNoBoss = 10 + 5 = 15
pesoTotalBoss = 0.50 + 0.35 + 0.20 + 0.10 = 1.15
```

### Progresión por oleada

| Oleada | `wavesDeProgreso` | Base que queda | Redistribución a bosses |
| ---: | ---: | ---: | ---: |
| 1–4 | 0 | 100% | 0% |
| 5 | 1 | 67.5% | 10% de lo redistribuido |
| 6 | 2 | 35% | 20% de lo redistribuido |
| 7 | 3 | 2.5% | 30% de lo redistribuido |
| 8 o posterior | 4+ | 0% | Aumenta hasta 100% en la oleada 14 |

La oleada 4 tiene una regla adicional: el primer spawn de esa oleada es un
evento de boss garantizado. Antes de crearlo se destruyen los enemigos activos.
En modo infinito el spawner continúa después de ese evento; en modo niveles,
el nivel queda pausado hasta confirmar la muerte del boss.

## Fórmulas de velocidad

En modo infinito, la oleada `w` usa:

```text
velocidadInicialOleada =
    velocidadInicial + incrementoVelocidadInicialPorOleada * max(0, w - 1)

incrementoPorEnemigo =
    incrementoVelocidad + incrementoIncrementoVelocidadPorOleada * max(0, w - 1)

velocidadMaximaOleada =
    velocidadMaxima + incrementoVelocidadMaximaPorOleada * max(0, w - 1)

velocidadEnemigo =
    min(
        velocidadInicialOleada + incrementoPorEnemigo * enemigosCreados,
        velocidadMaximaOleada
    )
```

Ejemplos con la configuración activa:

| Oleada | Velocidad inicial | Incremento por enemigo | Máximo |
| ---: | ---: | ---: | ---: |
| 1 | 5.00 | 0.05 | 10.00 |
| 2 | 5.25 | 0.06 | 10.50 |
| 3 | 5.50 | 0.07 | 11.00 |
| 4 | 5.75 | 0.08 | 11.50 |

`enemigosCreados` comienza en cero, por lo que el primer enemigo de cada
oleada usa la velocidad inicial de esa oleada.

## Fórmula de estadísticas

Al crear un enemigo, `EnemyStats` aplica multiplicadores de nivel y oleada:

```text
multiplicadorNivel = 1 + 0.25 * max(0, nivel - 1)
multiplicadorWave = 1 + 0.10 * max(0, oleada - 1)
multiplicadorTotal = multiplicadorNivel * multiplicadorWave

vida, daño, defensa y recompensa =
    valorBase * multiplicadorTotal
```

En el modo infinito actual el nivel permanece en 1, así que el multiplicador
depende únicamente de la oleada:

```text
multiplicadorTotal = 1 + 0.10 * (oleada - 1)
```

Los perfiles automáticos de pasiva se aplican después de estos multiplicadores.
Por ejemplo, un boss `Blindado` modifica su defensa con:

```text
defensaFinal = defensa * (1 + bonusPasiva * 2)
```
