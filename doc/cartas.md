# Tienda de torretas y pasivas

## Comprar y colocar torretas

`HUD > PanelCartas` muestra hasta cuatro ofertas de `TowerPlacer`. Al iniciar
no hay torretas compradas. El saldo inicial cubre las tres ofertas más baratas
(con un mínimo configurado de 150 engranajes). La primera oferta es Normal;
el resto se sortea por tipo y rareza. Las bajas
suman su recompensa al saldo, sin entregar torretas ni gastar automáticamente.

| Rareza | Precio inicial | Bono de daño |
|---|---:|---:|
| Normal | 50 | +0 % |
| Rara | 100 | +20 % |
| Épica | 150 | +50 % |
| Legendaria | 250 | +100 % |

En el Inspector del objeto **TowerPlacer**:

- **Precios Torretas** permite editar el costo por rareza (mínimo 1).
- **Puntos Actuales** es el saldo mínimo inicial (150 en `etapa1`); si las tres ofertas
  más baratas cuestan más, se aumenta automáticamente para cubrirlas.
- **Balance Torretas** configura tipos y rarezas iniciales/finales, oleada de
  balance final y bonos de daño. Los valores se normalizan entre candidatos;
  siguen vigentes las reglas de variedad y protección contra repeticiones.

Los valores de precio y daño se fijan al generar la oferta; cambiar el Inspector
afecta a las ofertas siguientes. La primera oferta Normal es la excepción al
sorteo de rareza. Los tipos con peso cero se excluyen. Si todos los pesos de tipo
son cero no se generan ofertas; un perfil de rarezas vacío se trata como Normal.

La carta muestra precio y daño. Si no alcanza el saldo, no permite comprar.
Pulsar una oferta asequible descuenta exactamente su precio, actualiza
`MarcadorDinero` y selecciona el preview. La carta pasa a **Comprada · colocar**.
Cancelar conserva esa compra, y volver a seleccionarla no cobra otra vez.
Puedes comprar otra oferta sin perder las compras anteriores.

Colocar en terreno válido consume solo la carta comprada y repone la tienda con
una oferta aleatoria nueva. No selecciona ni compra automáticamente la siguiente.
Camino, estructuras, clic en UI y posiciones inválidas no consumen la compra.
Ya no existe canje por 24 puntos ni una cola de torretas gratuitas.

## Elección de pasivas al derrotar al boss

El acceso lateral `HUD > PanelPasivas` permanece oculto mientras no haya pasivas.
Al elegir la primera, se habilita el icono y permite consultar las adquiridas
con un clic durante los niveles siguientes. Durante una elección de boss se
oculta temporalmente. El comando del editor deja oculto el acceso inicial.

El boss especial de la oleada 4 (modo niveles o infinito) se conecta a `EnemyStats.AlMorir`. Solo
su muerte por daño entrega la elección; llegar a la casa, la limpieza de enemigos
y los bosses aleatorios fuera de ese evento no la disparan. La muerte se registra una sola vez.

Al derrotarlo se abre una elección de **tres pasivas distintas** del catálogo
de `Assets/Resources/RecompensaPasivas.asset`. Se priorizan las no adquiridas y, si faltan, se completa
con las adquiridas. Con menos de tres definiciones válidas distintas se muestran
las disponibles. El catálogo de `etapa1` contiene las 20 pasivas positivas de `Assets/Data/Pasivas`.

La elección pausa la partida y bloquea el mundo. Pulsar una carta o su botón
**Elegir** entrega una copia y cierra la ventana, restaura el cursor y el tiempo,
y continúa con el siguiente nivel mediante el flujo existente del spawner.
Escape, clic derecho y el fondo no descartan la recompensa. Si se derrota al
boss mientras hay otro menú abierto, la recompensa espera hasta cerrarlo.

Cada boss entrega una copia, aunque la definición ya se posea; dentro de una
misma elección no se repiten opciones. Las copias se conservan en la colección
durante la partida. Al adquirir una pasiva se aplican automáticamente sus mejoras
a torretas actuales y futuras, incluidos los previews. El catálogo incluye mejoras de daño, ralentización, crítico, eliminación instantánea
y defensa, además de costos y riesgos de aparición. Ver [Crear pasivas](pasivas.md).

Para solicitar una elección desde otro flujo:

```csharp
pasivasHUD.OfrecerRecompensaBoss();
```

`Desbloquear(definicion)` sigue disponible para desbloqueos directos sin duplicar
definiciones. No hay persistencia entre partidas.

## Contador de engranajes

`HUD > MarcadorDinero` contiene `MarcadorDineroHUD` y su texto hijo
`CantidadEngranajes`. Muestra el saldo `puntosActuales` al activarse y escucha
`AlCambiarPuntos` después de bajas y compras, sin consultar cada frame.
Con 50 engranajes, comprar una Normal deja 0; una baja que da 10 lo deja en 10.
El inventario de materiales recogidos del suelo sigue siendo un sistema separado.

## Editor y verificación

**Tools > Cartas > Configurar torretas y pasivas** repara las referencias a la
fábrica, colocador e icono. Los diseños siguen en `Assets/Art/Cards`.

En Play Mode comprobar:

- Hay cuatro ofertas y saldo suficiente para comprar las tres más baratas.
- Comprar esas tres descuenta sus precios sin agotar el saldo antes de la tercera.
- Una baja suma su recompensa y comprar selecciona el preview.
- No se puede comprar una oferta sin saldo suficiente, ni colocar sin comprar.
- Ganar más de 24 engranajes no entrega cartas ni descuenta saldo.
- Cancelar o cambiar de carta conserva la compra; reseleccionar no vuelve a cobrar.
- Camino, estructuras y clics sobre UI no consumen la compra.
- Colocar consume esa carta y aparece una nueva oferta, con máximo cuatro visibles.
- El multiplicador del preview coincide con el de la torreta colocada.
- Abrir o cerrar pasivas no consume ni cobra cartas; Escape cierra primero las pasivas.

Para probar la recompensa del boss en Play Mode:

- Antes de adquirir pasivas el acceso lateral no es visible ni intercepta clics.
- Después de elegir una pasiva aparece el icono lateral; al pasar de nivel sigue
  visible y permite abrir y cerrar la colección, que incluye la elegida.
- Derrotar al boss final muestra tres opciones distintas y pausa el juego.
- Escape, clic derecho y el fondo no cierran ni gastan la elección.
- Elegir una sola opción entrega una copia, cierra y continúa el nivel.
- Matar un enemigo común o un boss aleatorio no abre la elección.
- La limpieza de enemigos y un boss que llega a la casa no dan pasivas.
- Con dos pasivas ya adquiridas siguen apareciendo tres opciones distintas.

### Diagnóstico de la elección de pasivas

La recompensa busca el controlador incluso si está desactivado; si la escena no
lo tiene, lo crea. Una configuración vacía carga los valores de
`Assets/Resources/RecompensaPasivas.asset`, sin depender de recargar la escena
que ya estaba abierta. La selección usa un Canvas propio sobre el HUD.

En Play, selecciona `PanelPasivas` y abre el menú del componente `PasivasHUD`:
**Probar selección de pasiva (en Play)** abre el mismo flujo sin repetir el nivel.
La consola registra `[PASIVAS]` al morir el boss por daño, encolar la recompensa
y abrir las opciones. Si el boss llega a la casa y desaparece, no cuenta como
una baja y no entrega pasivas. La recompensa espera si hay pausa u otro menú.

Las nuevas pasivas de eventos incluyen economía al construir/vender, curación,
auras, sangrado y efectos por bajas. Las siete negativas son assets asociables
en **Penalizaciones** y no se sortean solas. Ver [Crear pasivas](pasivas.md).

Las siete negativas ya están asociadas a pasivas positivas. Elegir una de esas
pasivas en la selección del boss activa su costo automáticamente. Las normales
siguen sin penalizaciones. Las parejas están descritas en `doc/pasivas.md`.


### Barra de compra y HUD

El HUD muestra hasta cuatro ofertas en una fila en la esquina inferior derecha.
Cada oferta muestra nombre, rareza y precio. Pasar el cursor abre una vista de
lectura encima sin mover el botón de compra. Las ofertas sin saldo suficiente
se atenúan al 35 %; ganar engranajes actualiza el precio y la opacidad sin
reconstruir la barra. Colocar una torreta repone la oferta consumida.

El marcador inferior izquierdo muestra el saldo de engranajes. El puntero
exterior a la izquierda del reloj indica la **etapa**, conservando la posición,
tamaño, pivote y rotación inicial ajustados en la escena. Las manillas interiores
son decorativas y quedan fijas. El componente `RelojOleadasHUD` conserva su
nombre por compatibilidad; `Grados Por Etapa` y `Grosor Puntero` permiten ajustar
el avance y el contorno. Los cambios de etapa se notifican desde `EnemySpawner`.

Comprobación manual pendiente en Play Mode: comprar y colocar ofertas, revisar
los precios y la opacidad al cambiar el saldo, derrotar bosses y elegir pasivas,
y comprobar la alineación del puntero al pasar a las etapas 2 y 3.
