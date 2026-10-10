# Tower Defense Project

## Cómo clonar el proyecto

Sigue estos pasos para clonar el repositorio y tener todo listo para trabajar:

```bash
# 1. Clona el repositorio normalmente
git clone git@github.com:Matias311/towee-defense.git

# 2. Entra al directorio
cd towee-defense

# 3. Instala Git LFS (necesario para los archivos grandes de texturas)
git lfs install

# 4. Descarga los archivos grandes (LFS)
git lfs pull
```

### ¿Por qué estos pasos?

- El archivo `Assets/AllSkyFree/Cold Sunset/Cold Sunset Equirect.png` pesa **76 MB** y está configurado para ser gestionado por **Git LFS** (Large File Storage).
- `git lfs install` configura tu entorno local para manejar archivos LFS.
- `git lfs pull` descarga el archivo grande automáticamente. En futuros `git clone` o `git pull`, LFS descargará el archivo automáticamente si tienes instalado git lfs.

### ¿Qué archivos son?

- **Archivos normales** (se descargan con git regular): escenas, scripts, prefabs, configuración.
- **Archivos LFS** (requieren `git lfs install/pull`): texturas HDRI como `Cold Sunset Equirect.png`.

### Trabajo en equipo

1. Cada integrante debe correr `git lfs install` una vez al configurar su máquina.
2. Al hacer `git pull`, Git LFS descargará automáticamente los archivos grandes si ya están trackeados.
3. Para agregar nuevas texturas grandes, usar: `git lfs track "ruta/al/archivo.png"`

¡Listo! Ya puedes abrir el proyecto en Unity y comenzar a trabajar.

## Organización del proyecto

```text
Assets/
├── Art/                 # Recursos visuales: entorno, materiales, sprites y torres
├── Audio/               # Música y mezcladores de audio
├── Prefabs/Enemies/     # Prefabs de enemigos
├── Scenes/              # Escenas del juego y menús
├── Scripts/             # Código del juego, agrupado por función
│   ├── Camera/
│   ├── Collection/
│   ├── Data/
│   ├── Enemies/
│   ├── Menu/
│   ├── Towers/
│   └── Utilities/
├── Settings/            # URP, perfiles e Input System
├── ThirdParty/          # Polytope Studio, NaaszArts y datos de ProBuilder
└── TextMesh Pro/         # Recursos de TMP en su ubicación original
```

`Packages/` y `ProjectSettings/` mantienen la configuración de Unity. `Library/`, `Temp/`, `Logs/` y `UserSettings/` son carpetas generadas localmente.

Al mover o renombrar recursos, mueve también su archivo `.meta` para conservar el GUID y las referencias de escenas y prefabs. Los recursos importados de terceros mantienen su estructura interna; en `Art/Towers/Torreta-model/Models/Turret00` a `Turret08`, cada modelo conserva juntos su OBJ, MTL y textura. La carpeta `Torreta-model` separa además `UI`, `Projectiles`, `Prefabs` y `Data`.
