using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class ConfigurarCartasHUD
{
    [MenuItem("Tools/Cartas/Configurar torretas y pasivas")]
    [MenuItem("Tools/Cartas/Configurar panel desplegable")]
    public static void Configurar()
    {
        CartasHUD baraja = Object.FindAnyObjectByType<CartasHUD>();
        if (baraja == null)
        {
            EditorUtility.DisplayDialog("Cartas", "Abre la escena etapa1 para configurar su HUD.", "Aceptar");
            return;
        }
        var fabrica = AssetDatabase.LoadAssetAtPath<CartaFactory>("Assets/Data/Cartas/FabricaCartas.asset");
        var datos = new SerializedObject(baraja);
        datos.FindProperty("fabrica").objectReferenceValue = fabrica;
        datos.FindProperty("colocador").objectReferenceValue = Object.FindAnyObjectByType<TowerPlacer>();
        datos.ApplyModifiedProperties();

        PasivasHUD pasivas = Object.FindAnyObjectByType<PasivasHUD>();
        if (pasivas == null)
        {
            var objeto = new GameObject("PanelPasivas", typeof(RectTransform), typeof(Image));
            Undo.RegisterCreatedObjectUndo(objeto, "Crear acceso a pasivas");
            objeto.transform.SetParent(baraja.GetComponentInParent<Canvas>().rootCanvas.transform, false);
            objeto.layer = baraja.gameObject.layer;
            pasivas = Undo.AddComponent<PasivasHUD>(objeto);
        }
        datos = new SerializedObject(pasivas);
        if (datos.FindProperty("configuracion").objectReferenceValue == null)
            datos.FindProperty("configuracion").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<RecompensaPasivasDatos>("Assets/Resources/RecompensaPasivas.asset");
        datos.FindProperty("fabrica").objectReferenceValue = AssetDatabase.LoadAssetAtPath<CartaFactory>(
            "Assets/Data/Cartas/FabricaPasivas.asset");
        Sprite icono = AssetDatabase.LoadAssetAtPath<Sprite>(
            "Assets/Art/Sprites/Gameplay/interfaceMenu/InterfazAlta/pasivas.png");
        datos.FindProperty("icono").objectReferenceValue = icono;
        datos.ApplyModifiedProperties();
        Undo.RecordObject(pasivas.transform, "Alinear acceso a pasivas");
        var rect = (RectTransform)pasivas.transform;
        rect.anchorMin = rect.anchorMax = new Vector2(0, .5f);
        rect.pivot = new Vector2(0, .5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(169, 227);
        Image imagen = pasivas.GetComponent<Image>();
        if (imagen == null) imagen = Undo.AddComponent<Image>(pasivas.gameObject);
        Undo.RecordObject(imagen, "Asignar imagen de pasivas");
        imagen.sprite = icono;
        imagen.preserveAspect = true;
        imagen.raycastTarget = false;
        imagen.enabled = false;
        EditorSceneManager.MarkSceneDirty(baraja.gameObject.scene);
        EditorSceneManager.MarkSceneDirty(pasivas.gameObject.scene);
        Selection.activeGameObject = pasivas.gameObject;
    }
}
