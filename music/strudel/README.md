# Música de Elevy · Strudel

Cuatro bocetos originales para escuchar y ajustar en el REPL. Usan osciladores,
ruido y FM de Strudel: no necesitan muestras, bancos ni archivos de audio.
Los timbres evocan teclas, cristal, flauta y madera; son instrumentos
sintetizados, no grabaciones acústicas. Las fuentes `.js` se conservan aquí;
los cuatro WAV exportados están en `Assets/Resources/MusicaEtapas/`, con la
reproducción automática por escena preparada. Consulta
[la guía de integración](../../doc/musica.md) para reemplazarlos o probarlos.

## Escuchar

1. Abre <https://strudel.cc/>.
2. Copia **todo** uno de los archivos `.js` y reemplaza el contenido del editor.
3. Pulsa **Play**. Detén y reinicia la reproducción para oír desde el comienzo.
4. Para cambiar de pieza, detén, reemplaza todo el código y vuelve a reproducir.

| Archivo | Escena | Tempo | Forma que se repite |
| --- | --- | --- | --- |
| `01-una-chispa-bajo-la-lluvia.js` | Descenso, lluvia y primer bot | 84 BPM | 32 compases, unos 91 s |
| `02-lo-que-las-sombras-recuerdan.js` | Refugio, sueño, sombras y runa | 84 BPM | 32 compases, unos 91 s |
| `03-luces-bajo-la-tierra.js` | Ciudad y segunda visita | 96 BPM | 32 compases, 80 s |
| `04-dos-luces-en-el-abismo.js` | Descenso con el bot legendario (propuesta) | 104 BPM | 32 compases, unos 74 s |

Cada ciclo equivale a un compás de cuatro negras. `setcps(BPM / 60 / 4)`
establece el tempo. Los arreglos son deterministas: melodía y entradas
se repiten, sin notas aleatorias.

## Controles

- `COMBATE = 0`: ambiente y exploración. `COMBATE = 1`: añade percusión y
  refuerza la amenaza del sueño. Puedes usar valores intermedios, como `0.5`.
- En el nivel 3, `COMPASION = 0` es la primera visita. Cambia a `1` para
  escuchar la ciudad con claridad y sumar la respuesta musical de Elevy.
  Usa solamente `0` o `1` para este control.
- Tras editar un control, vuelve a evaluar con **Play**. Los controles son
  manuales: todavía no reciben eventos del juego.
- `.postgain(0.7)` al final de cada archivo controla el volumen conjunto
  conservando el balance de las capas.

## Intención musical

El motivo de Elevy es **mi♭–sol–fa–re** (`eb g f d`). En el primer nivel
aparece suspendido; en el sueño se fragmenta; al aparecer la runa incorpora
mi♭ como reposo. La ciudad tiene su propia melodía en mi♭ mayor. En la
segunda visita, Elevy responde con su motivo sobre la armonía de la comunidad.

El nivel 1 introduce cristal tras cuatro compases y el bot tras ocho. Los
últimos cuatro reducen el arreglo para respirar antes de repetir. El nivel 2
dedica los últimos ocho compases a la runa: es una demostración narrativa
que se repite con el bucle, no una transición disparada al ganar. El nivel 3
deja oír el tema de la comunidad en la primera visita y lo presenta más
abiertamente en el regreso. La culpa, el reinicio y el nacimiento del bot
legendario aún no tienen piezas independientes.

La etapa 4, **Dos luces en el abismo**, propone el primer descenso después
de la ciudad, con Elevy acompañada por el bot legendario. Esta ubicación es
provisional: el lore compartido todavía no describe una etapa 4. El motivo
de Elevy alterna con una nueva voz más grave para el compañero; luego ambas
se superponen. Un ostinato más firme sostiene el avance y los últimos cuatro
compases dejan espacio a los ecos minerales antes de repetir.

## Referencias técnicas

- Síntesis y FM: <https://strudel.cc/learn/synths/>
- Envolventes, filtros y efectos: <https://strudel.cc/learn/effects/>
- Entradas por compases con `mask`: <https://strudel.cc/learn/conditional-modifiers/>

## Comprobación

Las cuatro fuentes se evaluaron con el transpilador y el motor de patrones de
Strudel (`core`/`mini`/`transpiler` 1.2.6 y `webaudio` 1.3.0). Se consultaron
64 compases por variante, con combate en `0`, `0.5` y `1`, y ambas visitas
de la ciudad: 15 combinaciones. Se comprobaron notas, fuentes de sonido,
ganancias y repetición exacta de los eventos cada 32 compases.

La comprobación de patrones no sustituye una escucha: el balance final,
las colas de reverberación y la mezcla con los efectos del juego requieren
revisión auditiva antes de exportar las pistas definitivas.
