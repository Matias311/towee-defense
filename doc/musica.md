# Música de fondo por etapa

El reproductor `Assets/Scripts/Menu/MusicaEtapas.cs` se crea automáticamente
al iniciar Play, incluso si se abre una etapa directamente desde el editor.
Reproduce su canción en bucle, en estéreo 2D, cambia al cargar otra etapa y
se detiene al volver a los menús. Utiliza el grupo `musica` del MainMixer
asignado al AudioSettingsManager de cada etapa, por lo que los controles
existentes de volumen general y música también afectan estas canciones.
El volumen base del AudioSource es 0.5.

Los cuatro WAV ya están en `Assets/Resources/MusicaEtapas/`, con sus
metadatos y GUID originales. Se verificaron como PCM estéreo de 16 bits a
44100 Hz, con audio presente y duración de 32 compases. La carpeta temporal
`Assets/musica-new` se eliminó después de moverlos. Los `.js` de Strudel se
conservan como fuentes para futuras versiones de las composiciones.

## Exportar desde Strudel

1. Abre <https://strudel.cc/> y pega el contenido completo de una composición
   de `music/strudel`.
2. Para una primera versión de fondo deja `COMBATE = 0`. En la etapa 3 deja
   `COMPASION = 0` para la primera visita. Estos valores quedan grabados en
   el audio: cambiarlos en el `.js` después no modifica un WAV ya exportado.
3. Abre la pestaña **export**, que permite renderizar y descargar audio.
   Exporta **32 ciclos** desde el inicio: en estas piezas cada ciclo equivale
   a un compás. Si permite elegir formato, usa WAV estéreo.
4. Guarda cada descarga con el nombre indicado y cópiala a
   `Assets/Resources/MusicaEtapas/`.

| Composición | Escena que la reproduce | Archivo | Duración de 32 compases |
| --- | --- | --- | --- |
| `01-una-chispa-bajo-la-lluvia.js` | `etapa1` | `etapa1.wav` | 91.428571 s |
| `02-lo-que-las-sombras-recuerdan.js` | `etapa2` | `etapa2.wav` | 91.428571 s |
| `03-luces-bajo-la-tierra.js` | `etapa3` | `etapa3.wav` | 80 s |
| `04-dos-luces-en-el-abismo.js` | `etapa4` | `etapa4.wav` | 73.846154 s |

También se admiten archivos `.ogg`, con el mismo nombre base. No dejes WAV
y OGG de la misma etapa juntos: ambos tendrían la misma ruta de Resources.
El selector de etapas actualmente envía su primer botón a `etapa2` y tiene
`etapa1` como prueba. La asignación musical sigue el **nombre real de la
escena**, no el texto del botón.

## Importar y comprobar en Unity

1. Espera a que Unity importe el script y los audios. No es necesario crear
   un GameObject ni asignar la música en cada escena.
2. Abre `etapa1` y pulsa Play: debe aparecer `MusicaEtapas` en
   DontDestroyOnLoad y sonar su pista. Comprueba también las otras tres.
3. Cambia de etapa: la canción anterior se detiene y comienza la nueva.
   Vuelve al menú para comprobar que deja de sonar la música de la etapa.
4. En pausa, abre Opciones y mueve los sliders de música y volumen general.
   La música continúa durante la pausa y responde a ambos controles.
5. Escucha una vuelta completa del bucle. Si el exportador añadió silencio
   final o una cola extra de reverberación, ajusta el archivo para que dure
   exactamente 32 compases. Las colas que cruzan el límite del bucle deben
   mezclarse con el inicio si se busca continuidad; evita un fade a silencio
   al final de cada vuelta.

Si falta un archivo, la consola indica su ruta con el prefijo `[MUSICA]`.
La ausencia de audio no impide jugar. Los archivos y sus rutas están
comprobados; queda verificar la reproducción, los cambios de escena y la
mezcla mediante Play en Unity.

Referencias: [exportar audio desde Strudel](https://strudel.cc/learn/faq/#how-to-record-or-export-audio),
[Resources.Load](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Resources.Load.html),
[AudioSource.loop](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/AudioSource-loop.html).
