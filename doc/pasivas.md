# Crear pasivas desde Unity

1. En Project, selecciona `Assets/Data/Pasivas` (o una de sus carpetas de rareza).
2. Clic derecho → **Create → Tower Defense → Pasiva**.
3. Completa **Título** y **Rareza**. El marco y el adorno se asignan automáticamente.
4. Marca **Todas Las Torretas** o desmárcalo y elige **Tipo Torreta**.
5. En **Mejoras**, pulsa **+**. Elige estadística y completa **Porcentaje**,
   **Cantidad Fija**, o ambos. Puedes agregar varias mejoras a una pasiva.
6. Abre `Assets/Resources/RecompensaPasivas.asset` y agrega la nueva pasiva a
   **Catalogo**. El próximo boss la incluirá entre las posibles recompensas.

El componente `PasivasHUD` permite asignar una **Configuracion** distinta si se
necesita otro catálogo. Por defecto carga el asset de Resources. La lista antigua
oculta de la escena solo se conserva como compatibilidad si falta la configuración.

## Ejemplo

Para una pasiva **Disparo preciso**:

- Rareza: Rara.
- Todas Las Torretas: activado.
- Mejora 1: Danio, Porcentaje 20, Cantidad Fija 0.
- Mejora 2: Penetracion, Porcentaje 0, Cantidad Fija 5.

Con **Descripcion Automatica** activada, el texto explica los efectos. Si se
quiere escribir una descripción propia, desactívalo y edita **Descripcion**.
El Inspector muestra una previsualización del texto final.

## Efectos disponibles

Daño, alcance, velocidad de ataque, defensa, penetración, radio de área y velocidad
de proyectil. El área tiene efecto en ataques de área y la velocidad de proyectil
en torretas que disparan proyectiles. La defensa conserva el comportamiento de
las estadísticas actuales; este sistema no agrega nuevas mecánicas de combate.

Al adquirir una pasiva se aplica permanentemente durante la partida, sin botón
Usar. Afecta torretas actuales, futuras y previews. El menú lateral se habilita
con la primera adquisición y permite consultar la colección.

Los valores se copian al adquirir: editar el asset durante Play afecta las
adquisiciones siguientes. No se modifican prefabs ni assets de estadísticas.
Recalcular una torreta no duplica sus bonos. Reiniciar la escena empieza otra
colección; no hay guardado entre partidas.

## Cómo se acumulan

Los porcentajes y cantidades fijas se suman por estadística y tipo de torreta:

`resultado = (valor base + suma de cantidades fijas) × (1 + suma de porcentajes / 100)`

El daño base ya incluye el multiplicador de rareza de la carta comprada.
Dos copias de +20 % dan +40 %, no +44 %. Una cantidad fija permite aumentar
estadísticas que empiezan en cero, como penetración.

La velocidad de ataque se calcula en **ataques por segundo**. Un bono de +20 %
divide el intervalo entre ataques por 1,2. Se conserva el mínimo de 0,05 segundos.

El catálogo del boss ahora contiene 20 pasivas positivas de `Assets/Data/Pasivas`.
Los cuatro assets anteriores de `Assets/Data/Cartas` se conservan por compatibilidad,
pero no forman parte del catálogo de recompensas.

## Verificación en Play

- Usar el menú del componente PasivasHUD → **Probar selección de pasiva (en Play)**.
- Elegir Fuerza mecánica y comprobar +10 % daño actual y en una torreta nueva.
- Elegir una segunda copia y comprobar que el bono pasa a +20 %.
- Crear una pasiva solo para Cañón y comprobar que no afecta otras torretas.
- Reconfigurar, cancelar o reseleccionar una torreta no acumula bonos adicionales.

## Primer catálogo

| Rareza | Pasiva | Beneficio | Costo o riesgo |
|---|---|---|---|
| Normal | Fuerza mecánica | +10 % daño | — |
| Normal | Freno suave | -5 % velocidad enemiga | — |
| Normal | Golpe afortunado | 5 % crítico | — |
| Rara | Fuerza reforzada | +15 % daño | — |
| Rara | Freno reforzado | -10 % velocidad enemiga | — |
| Rara | Sentencia | 10 % eliminación instantánea | — |
| Rara | Muralla | 20 % reducción de daño recibido por la casa | — |
| Épica | Poder sangriento | +20 % daño | Pierde 15 % de vida actual al elegirla |
| Épica | Precisión peligrosa | 20 % crítico | 10 % de un raro extra por aparición programada |
| Épica | Sentencia voraz | 20 % eliminación instantánea | +20 % frecuencia de aparición |
| Legendaria | Pacto del desafío | +50 % daño | 50 % de especial o jefe extra por oleada |
| Legendaria | Fortaleza acelerada | 40 % reducción de daño a la casa | +30 % velocidad enemiga |

### Valores iniciales elegidos para completar la propuesta

- Crítico: daño ×2 por impacto, después del bono de daño y antes de la defensa enemiga.
- Insta kill: probabilidad por enemigo impactado, también en área. No afecta a jefes
  por defecto; **Insta Kill Afecta Jefes** permite cambiarlo en el asset.
- Poder sangriento paga 15 % de la vida **actual**, una vez por adquisición. Ignora
  la defensa de la casa. El campo alternativo **Aumento Danio Recibido Casa** permite
  configurar mayor daño recibido permanente en lugar de ese costo.
- Para Sentencia voraz se eligió +20 % de frecuencia: los tiempos de espera del
  spawner se consumen ×1,2 más rápido. No cambia el número programado por oleada.
- El “30$” se interpretó como 30 % de velocidad.
- La vida extra legendaria queda como idea pendiente para EPE3; no está activada.

Todos esos valores se editan en **Combate** o **Mundo** dentro del asset de pasiva.
Las mejoras de Mundo son globales; el filtro de tipo de torreta solo afecta las
mejoras de estadísticas y Combate.

### Apariciones extra y acumulación

Precisión peligrosa tira su chance después de cada aparición programada. Agrega
un `UltraPro`, o `Special` si no hay UltraPro configurado. Pacto del desafío tira
su chance una vez en la primera aparición de cada oleada (también en la oleada
del boss), y elige entre los especiales y jefes configurados con prefab.

Los extras no generan otros extras, no avanzan el contador de oleada y sus jefes
no entregan otra elección de pasivas. Solo el boss del evento de la cuarta oleada
entrega la recompensa de pasivas. Si falta un prefab especial se avisa en consola.

Las copias suman sus chances hasta 100 %. Se limita la ralentización y la reducción
de daño de la casa a 90 % para evitar inmovilización o inmunidad completa. La
velocidad final combina `velocidad base × (1 + aumentos) × (1 - ralentización)`.
Por ejemplo, +30 % velocidad y -5 % ralentización dan velocidad ×1,235.

Para comprobar en Play: elegir una pasiva con cada efecto, verificar vida de la
casa al elegir Poder sangriento y al recibir golpes, probar disparo directo,
proyectil y área, y comprobar que matar un jefe extra no abre la recompensa.

## Nuevas pasivas por eventos

Se agregaron ocho positivas al catálogo del boss y siete penalizaciones reutilizables.
Las positivas ahora suman **20**: seis normales, cinco raras, cinco épicas y cuatro
legendarias. Los valores se editan en **Eventos** o **Combate** del asset.

| Rareza | Pasiva | Efecto |
|---|---|---|
| Normal | Tiro improbable | 1 % eliminación instantánea; excluye jefes y minijefes |
| Normal | Herida abierta | 5 % sangrado: 20 % del daño del golpe repartido en 4 segundos |
| Normal | Reparación paciente | Cada 30 segundos sin daño cura 1; máximo 5 por nivel |
| Rara | Ingeniería rentable | 15 % de reembolsar la construcción o duplicar la devolución de venta |
| Épica | Respuesta frenética | Al recibir daño en la casa, 25 % de atacar ×2 durante 5 segundos |
| Épica | Red de apoyo | 5 % de que una torreta nueva otorgue aura de +15 % daño y +25 % alcance |
| Legendaria | Pieza maestra | La próxima torreta realmente colocada recibe +200 % daño, una vez |
| Legendaria | Pulso glacial | Cada 10 bajas ralentiza 80 % a todos los enemigos durante 5 segundos |

### Construcción, venta y aura

Se añadió venta de torretas: apunta con el ratón a una torreta colocada y pulsa
**V**. Su panel muestra la devolución. Por defecto se devuelve el **50 % del precio
realmente pagado**, configurable en `TowerPlacer > Porcentaje Devolucion Venta`.
Una venta solo se procesa una vez y actualiza el contador de engranajes.

Ingeniería rentable tira su chance al **colocar**, no al comprar ni al mostrar el
preview. Si se activa devuelve el costo completo de esa construcción; al vender,
duplica la devolución normal. Venta sin retorno anula la devolución incluso si
está Ingeniería rentable activa. Cancelar/reseleccionar no genera dinero.

Pieza maestra se consume al confirmar una posición válida. Cancelar, una posición
inválida y los previews no consumen el bono. No vuelve a activarse al cambiar de
nivel; otra copia adquirida prepara otro bono para la próxima colocación.

Red de apoyo puede convertir cualquier torreta nueva en emisora de aura. Mantiene
su ataque y beneficia solo a **otras** torretas dentro de su rango sin auras.
Los radios de aura no se amplifican entre sí. Los bonos se retiran al vender o
desactivar el emisor; varias auras suman sus porcentajes. El panel de estadísticas
muestra si la torreta emite aura. Comprar un modelo con aspecto de soporte no es
un requisito; todavía no existe un tipo de torreta de soporte separado.

### Temporizadores, curación y sangrado

Los temporizadores usan tiempo del juego: se detienen al pausar y durante la
elección de pasiva. Frenesí no acumula ni refresca duración mientras está activo.
La represalia especial usa 10 segundos al 75 % de velocidad de ataque; durante
su actividad no refresca su duración. Con frenesí y represalia juntos, la velocidad
es ×1,5. El panel de torreta muestra el intervalo efectivo.

La curación no supera la vida máxima ni revive una casa destruida. Recibir daño
reinicia la espera de 30 segundos. El límite de 5 se reinicia al comenzar cada
nivel; duplicar la pasiva no duplica ese límite. Sacrificar vida al adquirir una
pasiva no cuenta como un ataque para activar Frenesí.

Sangrado reparte exactamente el daño total configurado e ignora defensa. Cada
aplicación conserva su temporizador; el último tick no supera la duración restante.
No activa nuevos sangrados por sí mismo. Una baja por sangrado cuenta para pulsos,
recompensas y el boss exactamente una vez.

Para excluir un minijefe de insta kill, marca **Es Mini Jefe** en su prefab o en
su `EnemyStatsData`. Los tipos BossL1–BossL4 están excluidos automáticamente salvo
que la pasiva permita afectar jefes explícitamente.

## Penalizaciones asociables

Los siete assets se encuentran en `Assets/Data/Pasivas/Negativas`, organizados por
severidad. **No se ofrecen solos ni se aplican por tenerlos en la carpeta.**

Para usar uno: selecciona una pasiva positiva, agrega una entrada en
**Penalizaciones** y arrastra el asset negativo. Adquirir la positiva también
activa el costo; los ciclos entre referencias se ignoran. Las parejas iniciales están asignadas según la tabla siguiente y se pueden
modificar desde Unity.

| Severidad | Penalización | Comportamiento |
|---|---|---|
| Normal | Venta sin retorno | Vender devuelve cero engranajes |
| Rara | Impacto brutal | Cada impacto contra la casa tiene 25 % de causar daño doble |
| Rara | Coraza de proyectiles | 20 % de enemigos obtiene un escudo de un proyectil |
| Épica | Inflación de guerra | Precio ×2 durante el siguiente nivel, luego vuelve a normal |
| Épica | Asedio continuo | Sin tiempo de descanso entre oleadas |
| Épica | Represalia especial | Matar Special o UltraPro tiene 75 % de reducir cadencia al 75 % por 10 segundos |
| Legendaria | Horda enfurecida | Cada 25 bajas, 50 % de duplicar velocidad enemiga hasta terminar ese nivel |

El escudo consume un punto para bloquear todo el impacto de un proyectil,
incluidos crítico, insta kill y sangrado de ese impacto. El siguiente proyectil
hace daño normalmente. Disparos directos sin proyectil y daños periódicos no
consumen escudos. La chance se sortea una sola vez por enemigo; adquirir otras
pasivas no vuelve a otorgarle el escudo ya gastado.

Inflación cambia las ofertas sin comprar; no vuelve a cobrar cartas ya compradas.
La venta y el reembolso usan el precio pagado. Horda enfurecida no se multiplica
repetidamente: permanece ×2 y se reinicia al pasar de nivel. Asedio continuo no
elimina oleadas ni modifica el tiempo normal entre enemigos, solo su descanso.

Las penalizaciones son permanentes mientras dure la partida salvo los efectos
con duración indicada. **Solo Un Nivel** mantiene una penalización adquirida al
final del boss durante el siguiente nivel y la retira al empezar el posterior.
Ese alcance temporal se aplica a los campos de Eventos; para costos temporales
usa esos campos, no mejoras permanentes de Mundo o estadísticas.

### Verificación adicional en Play

- Asociar una negativa a una positiva y usar la selección de prueba del HUD.
- Comprar/cancelar no consume Pieza maestra ni genera reembolsos.
- Colocar/vender muestra cambios correctos en el saldo; vender dos veces es imposible.
- Colocar un emisor de aura, comparar torretas dentro/fuera y venderlo para retirar bonos.
- Recibir daño activa Frenesí según la chance y corta la espera de curación.
- Las bajas 10 y 20 disparan pulso; las 25 y 50 pueden activar Horda enfurecida.
- Pausar conserva las duraciones restantes; avanzar de nivel reinicia contadores y límites.
- El primer proyectil contra un enemigo con escudo no baja vida; el segundo sí.

## Parejas iniciales de beneficios y penalizaciones

| Pasiva | Penalización asignada | Criterio |
|---|---|---|
| Ingeniería rentable | Venta sin retorno | Conserva el reembolso de construcción, pero exige comprometerse con la colocación |
| Sentencia | Coraza de proyectiles | La eliminación instantánea conserva su utilidad tras romper el escudo |
| Respuesta frenética | Impacto brutal | Mayor cadencia al recibir daño a cambio de impactos potencialmente más peligrosos |
| Red de apoyo | Represalia especial | El aura permanente compensa períodos de menor cadencia tras bajas especiales |
| Sentencia voraz | Asedio continuo | Su 20 % de insta kill afronta oleadas sin descanso, además del +20 % de frecuencia existente |
| Pieza maestra | Inflación de guerra | El +200 % de la siguiente torreta se paga con precios dobles durante un nivel |
| Pulso glacial | Horda enfurecida | El control periódico del 80 % compensa enemigos más rápidos entre pulsos |

Las seis normales quedan sin negativas. Las pasivas que ya tenían otros costos
conservan esos valores; en Sentencia voraz se añade Asedio continuo al costo de
frecuencia existente. Cada pareja usa referencias de assets, sin duplicar el
código ni los datos negativos. El texto de la carta muestra el efecto y duración
de la penalización, no solo su nombre. Los ciclos de referencias también se
ignoran al construir la descripción.
