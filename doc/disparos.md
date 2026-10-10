# Disparos de torretas

Las torres colocadas reciben un `TowerAttackController`. La previsualización de colocación no dispara.

En el asset `TowerStatsData` asignado a la entrada de `TowerPlacer`, configura:

- `usarProyectil`: activa el proyectil visible; desactivado aplica daño instantáneo.
- `prefabProyectil`: aspecto del disparo. El asset `tower_base_stats` usa la flecha reparada. Si queda vacío, se genera una esfera amarilla.
- `velocidadProyectil`: unidades por segundo; se respeta sin aumentarla según la velocidad del enemigo. Debe ser suficiente para alcanzar enemigos en movimiento.
- `tiempoEntreAtaques`: intervalo entre disparos.
- `tipoAtaque`, `radioArea` y `cantidadObjetivos`: configuran daño directo o de área. El daño de área incluye primero al enemigo alcanzado y no cuenta varias veces sus colliders.

Para fijar la salida exactamente en el cañón, crea un hijo vacío en la boca del cañón y asígnalo a `puntoDisparo` de `TowerAttackController` en el prefab de la torre. Sin asignación, sale por encima del modelo visible. El alcance se mide desde el centro visual empleado por el indicador de rango.

Los proyectiles con `SpriteRenderer` miran hacia la cámara principal y rotan dentro de su plano según su trayectoria. `offsetAnguloSprite` permite adaptar imágenes que no apuntan hacia la derecha. Los proyectiles 3D mantienen su rotación visual relativa y apuntan hacia el objetivo.

## Comprobación en Unity

Abre `etapa1`, inicia Play y coloca una torre cerca del camino. Comprueba que la flecha sale del modelo, es visible en las cámaras disponibles y reduce la vida al impactar. Cambia la velocidad y el intervalo en el asset antes de iniciar Play para comprobar sus efectos. Prueba también un ataque de área con varios enemigos, incluyendo un enemigo con varios colliders, y un prefab sin proyectil asignado para comprobar la esfera de respaldo.

## Impactos visibles

Cada impacto genera un destello radial que se expande y desvanece en 0,3 segundos. Los ataques de área añaden una onda horizontal que alcanza el radio de daño configurado y dura 0,45 segundos. El efecto vive separado del proyectil y del enemigo, por lo que permanece visible si el golpe mata al enemigo. El destello se orienta hacia la cámara y se desplaza hasta delante del volumen del enemigo para evitar quedar oculto. Los ataques instantáneos también muestran el impacto.

Comprueba un golpe normal, uno letal y uno de área en Play, alternando entre las cámaras. Los proyectiles que pierden su objetivo o agotan su tiempo de vida desaparecen sin destello de impacto.

## Visibilidad durante el recorrido

`TowerProjectile` aumenta el tamaño visual por defecto a 1,5 veces y aplica un tamaño mínimo de 1,2 unidades a los sprites. Añade una estela dorada de 0,14 unidades de ancho y 0,16 segundos de duración, que se desvanece después de desaparecer la bala. Si el prefab ya contiene un TrailRenderer, conserva esa estela. Estos ajustes no cambian el daño, la velocidad ni el radio usado para detectar el impacto.

Para personalizar cada proyectil, añade `TowerProjectile` a su prefab y ajusta `multiplicadorVisual`, `tamanoMinimoSprite`, `mostrarEstela`, `colorEstela`, `anchoEstela` y `duracionEstela` en el Inspector. Si el componente se añade automáticamente al disparar, usa los valores predeterminados.
