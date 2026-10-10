# Candidatos para limpieza — pendiente de confirmación

Limpieza ejecutada con confirmación del usuario: grupos 1, 3, 4 y 8 eliminados junto con sus .meta. Grupos 2, 5, 6 y 7 conservados por petición explícita. Escenas adicionales y recursos de terceros sin cambios. Fecha: 9 de octubre de 2026.

Las listas siguientes documentan la revisión original; los grupos eliminados ya no están presentes.

Se analizaron referencias GUID transitivas en todas las escenas, assets, prefabs y archivos .meta; ProjectSettings; referencias relativas OBJ/MTL; rutas literales del código; Resources y shaders. Se conservaron todos los scripts como raíces porque varios componentes se agregan desde código. Es una revisión estática: un recurso sin referencias puede ser trabajo futuro o una herramienta de edición.

Las escenas habilitadas son MenuPrincipal, MenuJuego y etapa1. Las escenas adicionales se conservaron durante el análisis de dependencias, por lo que sus recursos no se propusieron como prescindibles si esas escenas los usan.

No borrar completas las carpetas de Polytope Studio, NaaszArts, TextMesh Pro, Audio, Settings ni los modelos Turret00, Turret04 y Turret07: contienen recursos utilizados. Se excluyeron los includes, shaders y licencias de TextMesh Pro de la propuesta. Los datos de ProBuilder se conservan como herramienta de edición.

Cada eliminación aprobada incluiría los .meta correspondientes.

## 1. Carpetas vacías

- `Assets/Scripts/UI`
- `Assets/ThirdParty/PolytopeRenderSettings`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/URP`

## 2. Modelos de torretas sin referencias

- `Assets/Art/Towers/Torreta-model/Models/Turret01/Turret(Tower_Defens)-1.mtl`
- `Assets/Art/Towers/Torreta-model/Models/Turret01/Turret(Tower_Defens)-1.obj`
- `Assets/Art/Towers/Torreta-model/Models/Turret01/Turret(Tower_Defens)-1.png`
- `Assets/Art/Towers/Torreta-model/Models/Turret02/Turret(Tower_Defens)-2.mtl`
- `Assets/Art/Towers/Torreta-model/Models/Turret02/Turret(Tower_Defens)-2.obj`
- `Assets/Art/Towers/Torreta-model/Models/Turret02/Turret(Tower_Defens)-2.png`
- `Assets/Art/Towers/Torreta-model/Models/Turret03/Turret(Tower_Defens)-3.mtl`
- `Assets/Art/Towers/Torreta-model/Models/Turret03/Turret(Tower_Defens)-3.obj`
- `Assets/Art/Towers/Torreta-model/Models/Turret03/Turret(Tower_Defens)-3.png`
- `Assets/Art/Towers/Torreta-model/Models/Turret05/Turret(Tower_Defens)-5.mtl`
- `Assets/Art/Towers/Torreta-model/Models/Turret05/Turret(Tower_Defens)-5.obj`
- `Assets/Art/Towers/Torreta-model/Models/Turret05/Turret(Tower_Defens)-5.png`
- `Assets/Art/Towers/Torreta-model/Models/Turret06/Turret(Tower_Defens)-6.mtl`
- `Assets/Art/Towers/Torreta-model/Models/Turret06/Turret(Tower_Defens)-6.obj`
- `Assets/Art/Towers/Torreta-model/Models/Turret06/Turret(Tower_Defens)-6.png`
- `Assets/Art/Towers/Torreta-model/Models/Turret08/Turret(Tower_Defens)-8.mtl`
- `Assets/Art/Towers/Torreta-model/Models/Turret08/Turret(Tower_Defens)-8.obj`
- `Assets/Art/Towers/Torreta-model/Models/Turret08/Turret(Tower_Defens)-8.png`

## 3. Efectos importados sin uso actual

El destello actual se genera por código con TowerImpactVisual; no utiliza estos spritesheets. El README corresponde a la licencia del pack y debe conservarse si se mantiene parte del pack.

- `Assets/Art/Towers/Torreta-model/Effects/10_weaponhit_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/11_fire_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/12_nebula_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/13_vortex_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/14_phantom_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/15_loading_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/16_sunburn_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/17_felspell_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/18_midnight_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/19_freezing_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/1_magicspell_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/20_magicbubbles_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/2_magic8_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/3_bluefire_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/4_casting_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/5_magickahit_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/6_flamelash_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/7_firespin_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/8_protectioncircle_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/9_brightfire_spritesheet.png`
- `Assets/Art/Towers/Torreta-model/Effects/README.txt`

## 4. Previews y texturas adicionales de torretas

- `Assets/Art/Towers/Torreta-model/Previews/All.png`
- `Assets/Art/Towers/Torreta-model/Previews/cover.png`
- `Assets/Art/Towers/Torreta-model/Textures/00.png`
- `Assets/Art/Towers/Torreta-model/Textures/01.png`
- `Assets/Art/Towers/Torreta-model/Textures/02.png`
- `Assets/Art/Towers/Torreta-model/Textures/03.png`
- `Assets/Art/Towers/Torreta-model/Textures/05.png`
- `Assets/Art/Towers/Torreta-model/Textures/06.png`
- `Assets/Art/Towers/Torreta-model/Textures/07.png`

## 5. Prefabs y materiales de enemigos antiguos

- `Assets/Art/Materials/Enemies/Cube-Boss.mat`
- `Assets/Art/Materials/Enemies/Cube-enemy.mat`
- `Assets/Art/Materials/Enemies/Cube-specia4.mat`
- `Assets/Art/Materials/Enemies/Cube-special1 3.mat`
- `Assets/Art/Materials/Enemies/Cube-special1.mat`
- `Assets/Art/Materials/Enemies/Cube-special3.mat`
- `Assets/Prefabs/Enemies/enemy-cube-boss.prefab`
- `Assets/Prefabs/Enemies/enemy-cube-special 2.prefab`
- `Assets/Prefabs/Enemies/enemy-cube-special 3.prefab`
- `Assets/Prefabs/Enemies/enemy-cube-special 4.prefab`
- `Assets/Prefabs/Enemies/enemy-cube-special.prefab`
- `Assets/Prefabs/Enemies/enemy-cube.prefab`

## 6. Recursos de interfaz y balas sin referencias

- `Assets/Art/Sprites/Gameplay/balas/Arrow01(100x100).png`
- `Assets/Art/Sprites/Gameplay/balas/Arrow01(32x32).png`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/CartasTorres/Dependencia carta epica1.psb`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/CartasTorres/carta_epica.png`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/CartasTorres/carta_legendaria.png`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/CartasTorres/carta_rara.png`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/CartasTorres/cartanormal.png`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/CartasTorres/dependencia_carta_epica2.png`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/CartasTorres/dependencia_legendaria.png`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/InterfazAlta/menu.png`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/InterfazAlta/oleadasreloj.png`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/InterfazAlta/pasivas.png`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/Marco/herido/New Animation.anim`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/Marco/herido/herido.controller`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/Marco/herido/herido.png`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/Marco/herido/herido.prefab`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/Marco/normal/New Animation.anim`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/Marco/normal/normal.anim`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/Marco/normal/normal.controller`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/Marco/normal/normal.prefab`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/vida/vida1.png`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/vida/vida2.png`
- `Assets/Art/Sprites/Gameplay/interfaceMenu/vida/vida3.png`

## 7. Capas y texturas de terreno sin referencias

- `Assets/Art/Environment/textura/NewLayer 1.terrainlayer`
- `Assets/Art/Environment/textura/NewLayer 2.terrainlayer`
- `Assets/Art/Environment/textura/NewLayer.terrainlayer`
- `Assets/Art/Environment/textura/pizo1.png`
- `Assets/Art/Environment/textura/pizo2.png`

## 8. Readme de la plantilla

- `Assets/Settings/Readme.asset`

## 9. Escenas adicionales (requieren decidir si se conservan)

- `Assets/Scenes/MenuPrincipal 2.unity`
- `Assets/Scenes/MenuPrincipal2.unity`
- `Assets/Scenes/etapa2.unity`
- `Assets/ThirdParty/NaaszArts/Fantasy House/Scenes/Fantasy House.unity`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Demos/Environment_Free/Environment_Free.unity`

## 10. Recursos de terceros para revisión individual

Lista para revisar individualmente; contiene modelos, prefabs, materiales y archivos auxiliares de packs importados. No autoriza eliminar los packs completos ni documentación/licencias de recursos que se conserven.

- `Assets/ThirdParty/NaaszArts/Fantasy House/Materials/B&W_Materials/B&W_Door_Metal.mat`
- `Assets/ThirdParty/NaaszArts/Fantasy House/Materials/B&W_Materials/B&W_Foundation.mat`
- `Assets/ThirdParty/NaaszArts/Fantasy House/Materials/B&W_Materials/B&W_Lantern_Metal.mat`
- `Assets/ThirdParty/NaaszArts/Fantasy House/Materials/B&W_Materials/B&W_Railing_Metal.mat`
- `Assets/ThirdParty/NaaszArts/Fantasy House/Materials/B&W_Materials/B&W_Textures/B&W_RailingTextured.png`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Demos/Environment_Free/Helpers/Ground_Layer_01.terrainlayer`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Demos/Environment_Free/Helpers/Ground_Layer_02.terrainlayer`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Demos/Environment_Free/Helpers/Plane mat.mat`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Flowers/PT_Poppy_02.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Mushrooms/PT_Caesars_Mushroom_01.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Plants/PT_Grass_02.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Rocks/PT_Generic_Rock_01.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Rocks/PT_Menhir_Rock_02.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Rocks/PT_Ore_Rock_01.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Rocks/PT_Ore_Rock_01_split.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Rocks/PT_River_Rock_Pile_02.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Shrubs/PT_Generic_Shrub_01_dead.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Shrubs/PT_Generic_Shrub_01_green.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Trees/PT_Fruit_Tree_01_apples.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Trees/PT_Fruit_Tree_01_dead.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Trees/PT_Fruit_Tree_01_dead_cut.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Trees/PT_Fruit_Tree_01_green.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Trees/PT_Fruit_Tree_01_green_cut.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Trees/PT_Fruit_Tree_01_logs.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Trees/PT_Fruit_Tree_01_pears.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Trees/PT_Fruit_Tree_01_plums.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Trees/PT_Fruit_Tree_01_stump.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Trees/PT_Pine_Tree_03_dead.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Trees/PT_Pine_Tree_03_dead_cut.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Trees/PT_Pine_Tree_03_green.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Trees/PT_Pine_Tree_03_green_cut.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Trees/PT_Pine_Tree_03_logs.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Prefabs/Trees/PT_Pine_Tree_03_stump.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Materials/PT_Fruit_Tree_Foliage_Mat.mat`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Materials/PT_Fruit_Tree_Opaque_mat.mat`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Materials/PT_Generic_Leaf_mat.mat`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Materials/PT_Generic_Tree_Leaves_mat.mat`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Materials/PT_Generic_Tree_Trunk_mat.mat`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Materials/PT_Mushrooms_mat.mat`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Materials/PT_Pine_Tree_Leaves_Mat.mat`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Materials/PT_Pine_Tree_Trunk_Mat.mat`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Materials/PT_Terrain_mat.mat`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Rocks/PT_Generic_Rock_01.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Rocks/PT_Menhir_Rock_02.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Rocks/PT_Ore_Rock_01.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Rocks/PT_Ore_Rock_01_split.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Shrubs/PT_Generic_Shrub_01_dead.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Shrubs/PT_Generic_Shrub_01_green.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Trees/PT_Fruit_Tree_01_apples.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Trees/PT_Fruit_Tree_01_dead.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Trees/PT_Fruit_Tree_01_dead_cut.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Trees/PT_Fruit_Tree_01_green.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Trees/PT_Fruit_Tree_01_green_cut.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Trees/PT_Fruit_Tree_01_logs.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Trees/PT_Fruit_Tree_01_pears.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Trees/PT_Fruit_Tree_01_plums.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Trees/PT_Fruit_Tree_01_stump.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Trees/PT_Pine_Tree_03_dead.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Trees/PT_Pine_Tree_03_dead_cut.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Trees/PT_Pine_Tree_03_green_cut.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Trees/PT_Pine_Tree_03_logs.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Meshes/Trees/PT_Pine_Tree_03_stump.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Textures/PT_Fruit_Tree_Flowers_01.png`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Textures/PT_Fruit_Tree_Leaves_01.png`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Textures/PT_Generic_Leaves_01.png`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Textures/PT_Generic_Tree_Leaves_01.png`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Textures/PT_Ground_Generic_03.png`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Textures/PT_Ground_Grass_Green_01.png`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Textures/PT_Mushrooms_01.png`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Textures/PT_Pine_Tree_01.png`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Environments/Sources/Textures/PT_Tree_Trunk_01.png`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Village/Prefabs/Modular/Fence/PT_Modular_Fence_Wood_01.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Village/Prefabs/Modular/Fence/PT_Modular_Fence_Wood_02.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Village/Prefabs/Modular/Fence/PT_Modular_Fence_Wood_03.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Village/Prefabs/Modular/Fence/PT_Modular_Gate_Wood_01.prefab`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Village/Sources/Meshes/Modular/Fence/PT_Modular_Gate_Wood_01.fbx`
- `Assets/ThirdParty/Polytope Studio/Lowpoly_Village/Sources/Textures/PT_Medieval_Buildings_Texture_01mask2.png`
