using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PasivaDatos))]
[CanEditMultipleObjects]
public class PasivaDatosEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (!serializedObject.isEditingMultipleObjects && ((PasivaDatos)target).descripcionAutomatica)
            DrawPropertiesExcluding(serializedObject, "m_Script", "efecto", "porcentaje", "descripcion");
        else DrawPropertiesExcluding(serializedObject, "m_Script", "efecto", "porcentaje");
        serializedObject.ApplyModifiedProperties();
        if (!serializedObject.isEditingMultipleObjects)
            EditorGUILayout.HelpBox(((PasivaDatos)target).Descripcion, MessageType.Info);
        EditorGUILayout.HelpBox(((PasivaDatos)target).esPenalizacion
            ? "Asocia este asset a la lista Penalizaciones de una pasiva positiva. No se ofrece como recompensa independiente."
            : "Se activa al adquirirla. Agrega este asset al catálogo del boss; puedes asignarle costos en Penalizaciones.", MessageType.None);
    }
}
