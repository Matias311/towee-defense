using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConfigurarCartasHUD
{
    [MenuItem("Tools/Cartas/Configurar textos del HUD")]
    [MenuItem("Tools/Cartas/Configurar panel desplegable")]
    public static void Configurar()
    {
        RectTransform panel = null;
        foreach (RectTransform candidato in Resources.FindObjectsOfTypeAll<RectTransform>())
        {
            if (candidato.name == "PanelCartas" && candidato.gameObject.scene.IsValid()
                && candidato.gameObject.scene.isLoaded && !EditorUtility.IsPersistent(candidato))
            {
                panel = candidato;
                break;
            }
        }

        if (panel == null)
        {
            EditorUtility.DisplayDialog("Cartas del HUD", "Abre la escena etapa1 para configurar sus cartas.", "Aceptar");
            return;
        }

        Undo.IncrementCurrentGroup();
        int grupo = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Configurar textos de las cartas");
        CartasHUD panelDesplegable = panel.GetComponent<CartasHUD>();
        if (panelDesplegable == null)
            panelDesplegable = Undo.AddComponent<CartasHUD>(panel.gameObject);
        var datosPanel = new SerializedObject(panelDesplegable);
        if (datosPanel.FindProperty("fabrica").objectReferenceValue == null)
            datosPanel.FindProperty("fabrica").objectReferenceValue =
                AssetDatabase.LoadAssetAtPath<CartaFactory>("Assets/Data/Cartas/FabricaCartas.asset");
        datosPanel.ApplyModifiedProperties();
        CartaUI seleccion = null;
        foreach (string nombre in new[] { "CartaNormal", "CartaRara", "CartaEpica", "CartaLegendaria" })
        {
            Transform carta = panel.Find(nombre);
            if (carta == null)
                continue;

            CartaUI ui = carta.GetComponent<CartaUI>();
            bool nueva = ui == null;
            if (nueva)
                ui = Undo.AddComponent<CartaUI>(carta.gameObject);

            TMP_Text titulo = CrearTexto(carta, "Titulo", .37f, .45f, 23, true);
            TMP_Text descripcion = CrearTexto(carta, "Descripcion", .24f, .36f, 20, false);
            TMP_Text estado = CrearTexto(carta, "Estado", .18f, .23f, 15, true);

            var datos = new SerializedObject(ui);
            datos.FindProperty("titulo").objectReferenceValue = titulo;
            datos.FindProperty("descripcion").objectReferenceValue = descripcion;
            datos.FindProperty("textoEstado").objectReferenceValue = estado;
            if (nueva)
                datos.FindProperty("nombre").stringValue = nombre.Replace("Carta", "Carta ");
            datos.FindProperty("rareza").enumValueIndex = nombre == "CartaLegendaria" ? 3 : nombre == "CartaEpica" ? 2 : nombre == "CartaRara" ? 1 : 0;
            datos.ApplyModifiedProperties();
            Undo.RecordObject(ui, "Actualizar carta");
            ui.Mostrar(ui.Nombre, ui.Efecto);
            seleccion = ui;
        }

        Undo.CollapseUndoOperations(grupo);
        EditorSceneManager.MarkSceneDirty(panel.gameObject.scene);
        if (seleccion != null)
        {
            Selection.activeGameObject = seleccion.gameObject;
            EditorGUIUtility.PingObject(seleccion.gameObject);
        }
    }

    private static TMP_Text CrearTexto(Transform carta, string nombre, float abajo, float arriba, float tamano, bool negrita)
    {
        Transform existente = carta.Find(nombre);
        if (existente != null && existente.TryGetComponent(out TMP_Text textoExistente))
            return textoExistente;

        var objeto = new GameObject(nombre, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        Undo.RegisterCreatedObjectUndo(objeto, "Crear texto de carta");
        objeto.transform.SetParent(carta, false);
        objeto.layer = carta.gameObject.layer;
        var rect = (RectTransform)objeto.transform;
        rect.anchorMin = new Vector2(.14f, abajo);
        rect.anchorMax = new Vector2(.86f, arriba);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        var texto = objeto.GetComponent<TextMeshProUGUI>();
        texto.font = TMP_Settings.defaultFontAsset;
        texto.fontSize = tamano;
        texto.enableAutoSizing = true;
        texto.fontSizeMin = 10;
        texto.fontSizeMax = tamano;
        texto.fontStyle = negrita ? FontStyles.Bold : FontStyles.Normal;
        texto.alignment = TextAlignmentOptions.Center;
        texto.color = new Color(.16f, .13f, .12f);
        texto.raycastTarget = false;
        texto.richText = false;
        texto.overflowMode = TextOverflowModes.Ellipsis;
        return texto;
    }
}
