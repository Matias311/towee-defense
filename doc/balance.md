# Balance inicial de torres y enemigos

Los assets editables están en `Assets/Data/NewToweeStats` y
`Assets/Data/Enemigos`. Seleccionar un asset en Unity permite cambiar sus
valores. Las cuatro escenas usan los mismos perfiles; la rareza de cada carta
elige automáticamente su tipo y su perfil. Es un punto de partida para
pruebas en Play Mode, no un balance final validado.

## Torres normales, antes de pasivas

| Torre | Daño por impacto | Intervalo | DPS por objetivo | Rango | Penetración | Rol |
|---|---:|---:|---:|---:|---:|---|
| Normal · Normal_Melee | 12 | 0,2 s | 60 | 4,5 | 0 | Cuerpo a cuerpo, rápido |
| Rara · Raro_Stunner | 7 | 0,75 s | 9,3 | 5,5 | 0 | Proyectil; ralentiza 40 % por 1,5 s |
| Épica · Epico_Francotirador | 65 | 1,5 s | 43,3 | 11 | 25 | Proyectil; prioriza objetivos con más vida |
| Legendaria · Legendario_MeleeArea | 120 | 1,5 s | 80 | 8 | 5 | Ataque directo en radio 3,5; hasta 8 objetivos |

El daño físico usa `100 / (100 + max(0, defensa - penetración))`.
Los DPS de la tabla ignoran armadura, viaje del proyectil y tiempo sin objetivo.
Los precios por rareza se mantienen: 50/100/150/250 engranajes. El rol y las
estadísticas base ya forman parte del perfil de rareza; las pasivas se suman
después.
No se usa el asset antiguo `tower_base_stats` en estas escenas, incluido su
valor local de prueba de 999 de daño.

## Enemigos antes del escalado

| Enemigo | Vida | Daño a base | Defensa | Engranajes | Velocidad relativa |
|---|---:|---:|---:|---:|---:|
| Normal | 60 | 5 | 0 | 10 | 1 |
| Orco | 150 | 8 | 30 | 18 | 0,85 |
| Demonio | 95 | 7 | 10 | 16 | 1,20 |
| Boss etapa 1 | 800 | 25 | 10 | 80 | 0,65 |
| Boss etapa 2 | 1100 | 28 | 15 | 100 | 0,62 |
| Boss etapa 3 | 1450 | 32 | 20 | 120 | 0,60 |
| Boss etapa 4 | 1850 | 36 | 25 | 160 | 0,58 |

El orco tiene Resistente (+15 % a su defensa). Los demás valores son directos:
no añaden los perfiles automáticos de los enemigos sin asset.

## Progresión

El multiplicador existente de vida, daño, defensa y recompensas es
`(1 + 0,25 × (etapa - 1)) × (1 + 0,10 × (oleada - 1))`.
La defensa del orco recibe además su pasiva. La recompensa se redondea al entero
más cercano. El multiplicador de llegada a la casa y las pasivas de la partida
siguen aplicándose.

| Boss en oleada 4 | Multiplicador | Vida resultante | Daño a base aproximado |
|---|---:|---:|---:|
| Etapa 1 | 1,30 | 1040 | 32,5 |
| Etapa 2 | 1,625 | 1787,5 | 45,5 |
| Etapa 3 | 1,95 | 2827,5 | 62,4 |
| Etapa 4 | 2,275 | 4208,75 | 81,9 |

Las oleadas mantienen 15/17/19/21 enemigos según etapa. La velocidad común
comienza en 3, aumenta 0,02 por aparición, tiene un máximo de 6 y conserva el
incremento de 15 % por etapa. Después se multiplica por la velocidad relativa
del asset: los bosses son lentos y el demonio es más rápido.

## Validación pendiente en juego

Probar una partida completa con distintas primeras compras. Observar cuántos
enemigos llegan a la base, cuántas torres se pueden comprar y cuánto tarda cada
boss en morir. Ajustar primero vida/recompensas/cadencia de aparición; después
las pasivas y rarezas. Las rutas tienen longitudes distintas y requieren prueba
real para confirmar la dificultad.
