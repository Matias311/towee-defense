# Crear cartas

La fabrica configurada esta en `Assets/Data/Cartas/FabricaCartas.asset`.
Comparte los disenos de las cuatro rarezas; cada carta conserva su propio titulo,
descripcion y estado visual. La fabrica no aplica efectos a torres o enemigos.

## Desde Unity

1. En Project, abre `Assets/Data/Cartas`.
2. Clic derecho > **Create > Cartas > Definicion de carta**.
3. Nombra el archivo y completa **Rareza**, **Titulo** y **Descripcion** en el Inspector.
4. Selecciona **HUD > PanelCartas** en la escena `etapa1`.
5. En **Cartas HUD > Cartas Adicionales**, agrega una entrada y arrastra tu carta.
6. Guarda la escena fuera de Play. En Play, pulsa el abanico para verla.

Las cuatro cartas originales siguen usando los campos de sus componentes Carta UI.
Las nuevas definiciones se suman despues y se muestran en paginas de cuatro, con
botones Anterior y Siguiente. Puedes reordenar la lista del Inspector.

Si tienes una version anterior de la escena abierta, ejecuta
**Tools > Cartas > Configurar panel desplegable** para conectar la fabrica.
Este comando conserva las cartas adicionales que ya hayas agregado.

## Desde codigo

Para mostrar una carta nueva en tu propio contenedor UI, asigna el asset
FabricaCartas a un campo `CartaFactory` y llama:

```csharp
CartaUI vista = fabrica.Crear(
    RarezaCarta.Epica,
    "Disparo veloz",
    "Aumenta la velocidad de ataque un 20%.",
    contenedorUI
);
vista.CambiarEstado(EstadoCarta.Bloqueada);
```

Tambien puedes usar una definicion guardada:

```csharp
CartaUI vista = fabrica.Crear(definicion, contenedorUI);
```

Para agregar una definicion al panel desplegable durante la partida, usa una
referencia al componente `CartasHUD` del PanelCartas:

```csharp
hud.Agregar(definicion);
```

Si el panel esta abierto, salta a la ultima pagina para mostrar la nueva carta.
Las adiciones durante Play duran esa partida; para guardarlas, edita la lista
de la escena fuera de Play. Cambiar una vista no modifica la definicion compartida.

Sube los archivos `.asset`, sus `.meta`, los scripts y la escena guardada al repo.
